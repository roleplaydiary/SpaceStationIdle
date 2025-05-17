import json
import logging

# ----------  настройка логирования ----------
logging.basicConfig(
    filename="upgrade_update.log",
    filemode="w",
    level=logging.INFO,
    format="%(asctime)s - %(levelname)s - %(message)s"
)

logging.info("Начата генерация upgrade_data_updated.json на основе upgrade_data_google.json")

# ----------  соответствие enum ----------
UPGRADE_ENUM = [
    "DepartmentUnlock",
    "DepartmentMaxCrew",
    "StationMaxCrew",
    "DepartmentCrewHire",
    "DepartmentMaxWorkstations",
    "DepartmentWorkstationAdd",
]
ENUM_INDEX = {name: idx for idx, name in enumerate(UPGRADE_ENUM)}

# ----------  utils ----------
def val_or_zero(x):
    if x in (None, ""):
        return 0.0
    try:
        return float(str(x).replace("\u00a0", "").replace(",", "."))
    except Exception:
        return 0.0

def enum_index(name: str) -> int:
    if name in ENUM_INDEX:
        return ENUM_INDEX[name]
    logging.warning(f"Неизвестный Type '{name}', используется 0")
    return 0

# ----------  загрузка исходного JSON ----------
try:
    with open("upgrade_data_google.json", "r", encoding="utf-8") as f:
        google_data = json.load(f)
    logging.info("upgrade_data_google.json успешно загружен")
except Exception as e:
    logging.error(f"Ошибка загрузки upgrade_data_google.json: {e}")
    raise

rows = google_data[1:]  # пропускаем строку‑заголовок

# ----------  формируем новые items ----------
new_items = []

for idx, row in enumerate(rows, start=2):          # start=2 — номер строки в исходнике
    upgrade_id = row.get("UNNAMED_K", "").strip()
    if not upgrade_id:
        logging.warning(f"[Строка {idx}] пропущена (нет upgradeId)")
        continue

    try:
        level = int(row.get("UNNAMED_O") or 1)
    except ValueError:
        level = 1
    try:
        value = float(row.get("UNNAMED_P") or level)
    except ValueError:
        value = level

    item = {
        "upgradeId": upgrade_id,
        "displayName": row.get("UNNAMED_L", "").strip(),
        "description": row.get("UNNAMED_M", "").strip(),
        "type": enum_index(row.get("UNNAMED_N", "").strip()),
        "level": level,
        "value": value,
        "cost": {
            "credits": val_or_zero(row.get("UNNAMED_D")),
            "researchPoints": val_or_zero(row.get("UNNAMED_E")),
            "resources": {
                "Phoron":  val_or_zero(row.get("UNNAMED_J")),
                "Metal":   val_or_zero(row.get("UNNAMED_F")),
                "Glass":   0.0,
                "Plastic": val_or_zero(row.get("UNNAMED_G")),
                "Gold":    val_or_zero(row.get("UNNAMED_H")),
                "Silver":  val_or_zero(row.get("UNNAMED_I")),
                "Uranium": 0.0
            }
        }
    }

    new_items.append(item)
    logging.info(f"[Строка {idx}] добавлен upgradeId '{upgrade_id}'")

# ----------  сохраняем результат ----------
output = {"items": new_items}

try:
    with open("upgrade_data_updated.json", "w", encoding="utf-8") as f:
        json.dump(output, f, indent=4, ensure_ascii=False)
    logging.info(f"Файл upgrade_data_updated.json сохранён, записей: {len(new_items)}")
except Exception as e:
    logging.error(f"Ошибка сохранения upgrade_data_updated.json: {e}")
    raise

print("✅ Генерация завершена — см. upgrade_update.log")
