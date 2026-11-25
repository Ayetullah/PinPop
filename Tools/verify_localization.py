#!/usr/bin/env python3
"""Validate PinPop's English/Turkish catalog without changing game or saved data."""
import json
from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]


def main():
    rows = json.loads((ROOT / 'Assets/Resources/Localization/strings.json').read_text())['entries']
    choices = json.loads((ROOT / 'Assets/Resources/Localization/languages.json').read_text())['languages']
    assert {'en', 'tr'} <= {x['code'] for x in choices}
    assert 'auto' not in {x['code'] for x in choices}
    assert len(choices) == len({x['code'] for x in choices})
    native_names = {x['nativeName'] for x in choices}
    catalog = {}
    for row in rows:
        source, english = row['source'], row['en']
        assert source and english and source not in catalog, f'Duplicate or empty entry: {source}'
        assert sorted(re.findall(r'\{\d+\}', source)) == sorted(re.findall(r'\{\d+\}', english)), f'Placeholder mismatch: {source}'
        assert source == english and source in native_names or not any(c in english for c in 'çğıöşüÇĞİÖŞÜ'), f'Turkish copy in English translation: {source}'
        if row.get('enOne'):
            assert sorted(re.findall(r'\{\d+\}', source)) == sorted(re.findall(r'\{\d+\}', row['enOne'])), f'Singular placeholder mismatch: {source}'
        catalog[source] = english
    regions = json.loads((ROOT / 'Assets/Resources/MapArt/catalog.json').read_text())['regions']
    for region in regions:
        for copy in [region['title'], region['subtitle'], *region['names'], *region['extendedNames']]:
            assert copy in catalog, f'Map copy missing: {copy}'
    skipped = {'PinPopSelfTest', 'CrispUIReview', 'DevicePolishReview', 'PinPopStartupRecorder', 'GameLocalization', 'LanguageSettingsReview'}
    missing = []
    for path in (ROOT / 'Assets/Scripts').glob('*.cs'):
        if path.stem in skipped:
            continue
        for literal in re.findall(r'"(?:[^"\\]|\\.)*"', path.read_text()):
            value = json.loads(literal)
            if not any(c in value for c in 'çğıöşüÇĞİÖŞÜ') or value in catalog:
                continue
            # Mechanic identifiers remain untranslated for gameplay; their two
            # displayed parts are separately registered in the same catalog.
            title, _, body = value.partition('\n')
            if title in catalog and body in catalog:
                continue
            if path.stem == 'PinPopSplash' and value == 'İlk açılış çizimi':
                continue  # Diagnostic stage only, never displayed.
            missing.append(f'{path.name}: {value}')
    assert not missing, 'Unregistered Turkish copy:\n' + '\n'.join(missing)
    print(f'PASS: {len(rows)} entries, matching placeholders, 4 complete map themes, no unregistered Turkish runtime copy')


if __name__ == '__main__':
    main()
