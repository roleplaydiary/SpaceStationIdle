import json
import logging

# Настройка логирования
logging.basicConfig(
    filename="upgrade_update.log",
    filemode="w",
    level=logging.INFO,
    format="%(asctime)s - %(levelname)s - %(message)s"
)

logging.info("Начата генерация нового upgrade_data_updated.json на основе upgrade_data_google.json")

# Загрузка данных из google json
try:
    with open("upgrade_data_google.json", "r", encoding="utf-8") as f:
        google_data = json.load(f)
    logging.info("Файл upgrade_data_google.json успешно загружен")
except Exception as e:
    logging.error(f"Ошибка загрузки upgrade_data_google.json: {e}")
    raise

rows = google_data[1:]  # пропускаем заголовок

# Функция безопасного преобразования
def val_or_zero(x):
    if x is None or x == "":
        return 0.0
    try:
        return float(x)
    except:
        return 0.0

# Создание нового списка апгрейдов
new_items = []

for idx, row in enumerate(rows):
    upgrade_id = row.get("UNNAMED_K")
    if not upgrade_id:
        logging.warning(f"[Строка {idx}] Пропущена: отсутствует upgradeId")
        continue

    # Берём level и value из столбцов, если пусто - ставим дефолт 1
    try:
        level = int(row.get("UNNAMED_O")) if row.get("UNNAMED_O") not in (None, "") else 1
    except:
        level = 1
    try:
        value = float(row.get("UNNAMED_P")) if row.get("UNNAMED_P") not in (None, "") else 1
    except:
        value = 1

    item = {
        "upgradeId": upgrade_id,
        "displayName": row.get("UNNAMED_L") or "",
        "description": row.get("UNNAMED_M") or "",
        "type": row.get("UNNAMED_N") or "",
        "level": level,
        "value": value,
        "cost": {
            "credits": val_or_zero(row.get("UNNAMED_D")),
            "researchPoints": val_or_zero(row.get("UNNAMED_E")),
            "resources": {
                "Phoron": val_or_zero(row.get("UNNAMED_J")),
                "Metal": val_or_zero(row.get("UNNAMED_F")),
                "Glass": 0.0,
                "Plastic": val_or_zero(row.get("UNNAMED_G")),
                "Gold": val_or_zero(row.get("UNNAMED_H")),
                "Silver": val_or_zero(row.get("UNNAMED_I")),
                "Uranium": 0.0
            }
        }
    }

    new_items.append(item)
    logging.info(f"[Строка {idx}] Добавлен upgradeId '{upgrade_id}'")

# Сохраняем новый JSON
output = {"items": new_items}

try:
    with open("upgrade_data_updated.json", "w", encoding="utf-8") as f:
        json.dump(output, f, indent=4, ensure_ascii=False)
    logging.info("Файл upgrade_data_updated.json успешно сохранён")
except Exception as e:
    logging.error(f"Ошибка при сохранении upgrade_data_updated.json: {e}")
    raise

logging.info(f"Готово. Всего записей: {len(new_items)}")
print("Генерация завершена. Подробности в upgrade_update.log")
