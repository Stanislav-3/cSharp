1. Converter(библиотека классов)
Универсальный парсер XML и Json.
string SerializeJson(object), string SerializeXML(object) - методы для сериализации объекта в формат Json и XML. 
Поддерживаются любые объекты и типы. Атрибуты JsonIgnore, XMLIgnore предназначены для игнорирования полей и свойств класса 
при сериализации.
T DeserializeJson<T>(string), T DeserializeXML<T>(string) - методы для преобразования строки в формате Json и XML. 
Строка может быть преобразована любой тип.
2. ETL(Windows-служба, использующая Converter.dll)
При запуске при помощи класса OptionsManager загружает файлы конфигурации config.xml и appsettings.json. Если программа находит оба эти файла,
то настроки загружаются из appsettings.json. Если этот файл не найден или он не соответствует формату json, то загружается config.xml.
Если не удается использовать и appsettings.json, и config.xml, то используются стандартные настройки, а в месте расположения .exe создаются 
оба этих файла со стандартными настройками.
Загрузкой настроек занимается класс OptionsManager, имеющий метод Options GetOptions<T>(). 
Все настройки логически разбиты на классы:
MovingOptions - настройки путей к папкам(sourceDir, targetDir, enabling logging and additional archive)
ArchiveOptions - настройки архивирования(степень сжатия)
WatcherOptions - настройка мониторинга(фильтр)
Также все настройки после загрузки из файла конфигурации проходят валидацию(длина ключа, корректность путей и т.д.)
