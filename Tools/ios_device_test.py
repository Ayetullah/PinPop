#!/usr/bin/env python3
"""PinPop's local iOS device workflow. Never requests Apple passwords or stores credentials."""
import argparse
import json
import os
from pathlib import Path
import plistlib
import re
import shutil
import subprocess
import tempfile

ROOT = Path(__file__).resolve().parents[1]
EXPORT = ROOT / 'Builds/iOSDevice'
DERIVED = ROOT / 'Builds/iOSDerivedData'
TOOLS = Path.home() / 'Library/PinPop/iOS-tools/bin'
BUNDLE = 'com.pinpop.colorbowling'
CONFIG = 'Debug'


def run(args, env=None, capture=False):
    return subprocess.run([str(x) for x in args], env=env, text=True, capture_output=capture, check=True)


def environment():
    env = os.environ.copy()
    env['PATH'] = str(TOOLS) + os.pathsep + env.get('PATH', '')
    if not env.get('DEVELOPER_DIR'):
        selected = subprocess.run(['xcode-select', '-p'], capture_output=True, text=True).stdout.strip()
        candidates = [Path(selected)] + sorted(Path('/Applications').glob('Xcode*.app/Contents/Developer'))
        for candidate in candidates:
            if (candidate / 'usr/bin/xcodebuild').is_file():
                env['DEVELOPER_DIR'] = str(candidate)
                break
    return env


def xcode_ready(env):
    return bool(env.get('DEVELOPER_DIR') and (Path(env['DEVELOPER_DIR']) / 'usr/bin/xcodebuild').is_file())


def devices(env):
    if not xcode_ready(env):
        return []
    with tempfile.TemporaryDirectory(prefix='pinpop-device-list-') as tmp:
        output = Path(tmp) / 'devices.json'
        run(['xcrun', 'devicectl', 'list', 'devices', '--json-output', output], env, True)
        data = json.loads(output.read_text())
        return [d for d in data.get('result', {}).get('devices', []) if d.get('hardwareProperties', {}).get('platform') in ('iOS', 'iPadOS')]


def signed_device_app(path):
    if not (path / 'embedded.mobileprovision').is_file():
        return False
    return subprocess.run(['codesign', '--verify', '--deep', '--strict', str(path)],
                          capture_output=True).returncode == 0


def doctor(env):
    full_xcode = xcode_ready(env)
    identities = subprocess.run(['security', 'find-identity', '-v', '-p', 'codesigning'], capture_output=True, text=True).stdout
    match = re.search(r'(\d+) valid identities found', identities)
    connected, error = [], None
    try:
        connected = devices(env)
    except (subprocess.CalledProcessError, ValueError) as exc:
        error = str(exc)
    pod = shutil.which('pod', path=env['PATH'])
    usb_result = subprocess.run(['/usr/sbin/ioreg', '-p', 'IOUSB', '-l', '-w', '0'], capture_output=True, text=True)
    usb_detected = bool(re.search(r'"USB Product Name" = "(?:iPhone|iPad)"', usb_result.stdout)) if usb_result.returncode == 0 else None
    report = {
        'fullXcodeInstalled': full_xcode,
        'developerDirectory': env.get('DEVELOPER_DIR'),
        'cocoaPodsInstalled': bool(pod),
        'cocoaPodsExecutable': pod,
        'validSigningIdentities': int(match[1]) if match else 0,
        'deviceDiscoveryAvailable': full_xcode,
        'usbIOSDeviceDetected': usb_detected,
        'visibleIOSDevices': len(connected) if full_xcode else None,
        'deviceDiscoveryError': error,
        'xcodeProjectExported': (EXPORT / 'Unity-iPhone.xcodeproj/project.pbxproj').is_file(),
        'bundleId': BUNDLE,
        'deviceAppArtifactPresent': bool(list((DERIVED / ('Build/Products/' + CONFIG + '-iphoneos')).glob('*.app'))),
        'signedDeviceAppPresent': any(signed_device_app(p) for p in (DERIVED / ('Build/Products/' + CONFIG + '-iphoneos')).glob('*.app')),
    }
    if pod:
        report['cocoaPodsVersion'] = run([pod, '--version'], env, True).stdout.strip()
    if full_xcode:
        report['xcodeVersion'] = run(['xcodebuild', '-version'], env, True).stdout.strip()
    print(json.dumps(report, indent=2, ensure_ascii=False))
    out = ROOT / 'UserSettings/ios-device-host.json'
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(json.dumps(report, indent=2, ensure_ascii=False))
    return 0 if full_xcode and connected and report['validSigningIdentities'] > 0 else 2


def export(env):
    version = re.search(r'm_EditorVersion: (\S+)', (ROOT / 'ProjectSettings/ProjectVersion.txt').read_text())[1]
    unity = Path('/Applications/Unity/Hub/Editor') / version / 'Unity.app/Contents/MacOS/Unity'
    if not unity.is_file():
        raise RuntimeError('Unity Editor not found: ' + str(unity))
    cache = ROOT / 'Builds/.ios-export-cache'
    cache.mkdir(parents=True, exist_ok=True)
    for name in ('Assets', 'Packages', 'ProjectSettings'):
        if (cache / name).exists():
            shutil.rmtree(cache / name)
        shutil.copytree(ROOT / name, cache / name)
    env['PINPOP_IOS_RELEASE_PATH' if CONFIG == 'Release' else 'PINPOP_IOS_EXPORT_PATH'] = str(EXPORT)
    log = ROOT / 'Logs/ios-device-export.log'
    log.parent.mkdir(parents=True, exist_ok=True)
    run([unity, '-batchmode', '-buildTarget', 'iOS', '-projectPath', cache,
         '-executeMethod', 'IOSDeviceBuild.ExportReleaseCandidate' if CONFIG == 'Release' else 'IOSDeviceBuild.Export', '-quit', '-logFile', log], env)
    for name in (('ios-device-settings-verification.json', 'ios-device-export-verification.json', 'ios-release-candidate.json') if CONFIG == 'Release' else ('ios-device-settings-verification.json', 'ios-device-export-verification.json')):
        shutil.copy2(cache / 'Documentation' / name, ROOT / 'Documentation' / name)
    print('Xcode project exported:', EXPORT)


def device_id(args, env):
    if args.device:
        return args.device
    found = devices(env)
    if len(found) != 1:
        raise RuntimeError('Connect and unlock one iPhone, trust this Mac, enable Developer Mode, or supply --device IDENTIFIER.')
    return found[0]['identifier']


def app():
    found = list((DERIVED / ('Build/Products/' + CONFIG + '-iphoneos')).glob('*.app'))
    if len(found) != 1:
        raise RuntimeError('Signed device app is missing. Run the build command first.')
    if not signed_device_app(found[0]):
        raise RuntimeError('Device app is unsigned or has no provisioning profile. Complete a signed build before installation.')
    return found[0]


def build(args, env):
    if not xcode_ready(env):
        raise RuntimeError('Full Xcode is not installed. Command Line Tools cannot build the iOS app.')
    if not (EXPORT / 'Unity-iPhone.xcodeproj').is_dir():
        raise RuntimeError('Export the Unity iOS project first.')
    team = args.team or env.get('PINPOP_APPLE_TEAM_ID')
    if not team:
        settings = (ROOT / 'ProjectSettings/ProjectSettings.asset').read_text()
        match = re.search(r'^  appleDeveloperTeamID: (\S+)', settings, re.M)
        team = match[1] if match else None
    if not team:
        pbx = (EXPORT / 'Unity-iPhone.xcodeproj/project.pbxproj').read_text()
        teams = set(re.findall(r'DEVELOPMENT_TEAM\s*=\s*"?([A-Z0-9]{10})"?\s*;', pbx))
        if len(teams) == 1:
            team = teams.pop()
    if not team:
        raise RuntimeError('Select your Apple account/Team in Xcode, then supply --team TEAM_ID. Do not enter Apple passwords in this tool.')
    workspace = EXPORT / 'Unity-iPhone.xcworkspace'
    project = ['-workspace', workspace] if workspace.exists() else ['-project', EXPORT / 'Unity-iPhone.xcodeproj']
    if args.generic:
        destination = 'generic/platform=iOS'
    else:
        identifier = device_id(args, env)
        # xcodebuild uses the hardware UDID; devicectl uses a CoreDevice UUID.
        for device in devices(env):
            if identifier in (device.get('identifier'), device.get('hardwareProperties', {}).get('udid')):
                identifier = device.get('hardwareProperties', {}).get('udid') or identifier
                break
        destination = 'id=' + identifier
    run(['xcodebuild', *project, '-scheme', 'Unity-iPhone', '-configuration', CONFIG,
         '-destination', destination, '-derivedDataPath', DERIVED,
         '-allowProvisioningUpdates', '-allowProvisioningDeviceRegistration', 'DEVELOPMENT_TEAM=' + team, 'build'], env)
    with (app() / 'Info.plist').open('rb') as file:
        if plistlib.load(file).get('CFBundleIdentifier') != BUNDLE:
            raise RuntimeError('Unexpected bundle ID in signed device app.')
    print('Signed device app:', app())


def store_check(env):
    # Apple minimum SDK requirement effective 2026-04-28:
    # https://developer.apple.com/news/upcoming-requirements/?id=04282026a
    candidate = app()
    with (candidate / 'Info.plist').open('rb') as file:
        info = plistlib.load(file)
    sdk = info.get('DTSDKName', '')
    match = re.search(r'iphoneos(\d+)', sdk)
    profile = plistlib.loads(run(['security', 'cms', '-D', '-i', candidate / 'embedded.mobileprovision'], env, True).stdout.encode())
    distribution = not profile.get('ProvisionedDevices') and not profile.get('ProvisionsAllDevices', False) and not profile.get('Entitlements', {}).get('get-task-allow', True)
    report = {'sdk': sdk, 'minimumAppStoreSDKMajor': 26, 'sdkEligible': bool(match and int(match[1]) >= 26),
              'appStoreDistributionProfile': distribution, 'testAdsIntentionallyRetained': True,
              'source': 'https://developer.apple.com/news/upcoming-requirements/?id=04282026a'}
    report['readyForAppStoreUpload'] = report['sdkEligible'] and distribution
    print(json.dumps(report, indent=2))
    return 0 if report['readyForAppStoreUpload'] else 2


def main():
    global EXPORT, DERIVED, CONFIG
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('command', choices=['doctor', 'export', 'build', 'install', 'launch', 'store-check'])
    parser.add_argument('--team', help='Apple Team ID, not an account password')
    parser.add_argument('--device', help='Device identifier from Xcode/devicectl')
    parser.add_argument('--release', action='store_true', help='Use the non-development release candidate; test ads remain enabled')
    parser.add_argument('--generic', action='store_true', help='Build without a connected device')
    args = parser.parse_args()
    if args.release:
        CONFIG = 'Release'
        EXPORT = ROOT / 'Builds/iOSReleaseCandidate'
        DERIVED = ROOT / 'Builds/iOSReleaseDerivedData'
    env = environment()
    try:
        if args.command == 'store-check':
            return store_check(env)
        if args.command == 'doctor':
            return doctor(env)
        if args.command == 'export':
            export(env)
        elif args.command == 'build':
            build(args, env)
        else:
            if not xcode_ready(env):
                raise RuntimeError('Install full Xcode before installing or launching the app.')
            identifier = device_id(args, env)
            if args.command == 'install':
                run(['xcrun', 'devicectl', 'device', 'install', 'app', '--device', identifier, app()], env)
            else:
                run(['xcrun', 'devicectl', 'device', 'process', 'launch', '--device', identifier, BUNDLE], env)
        return 0
    except (RuntimeError, subprocess.CalledProcessError, OSError) as exc:
        print(str(exc))
        return 1


if __name__ == '__main__':
    raise SystemExit(main())
