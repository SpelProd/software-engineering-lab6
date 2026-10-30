cmake_minimum_required(VERSION 3.8)
project(StatsSolution LANGUAGES NONE)

# Подключаем подпроекты (они остаются без изменений)
add_subdirectory(StatsLib)
add_subdirectory(MainApp)

# =========================================================================
# ПРАВИЛА УСТАНОВКИ И КОМПОНЕНТЫ (В соответствии с правилами Linux) Изменение 1
# =========================================================================

# 1. Компонент "Программа" (Исполняемый файл в /usr/bin)
install(FILES "${CMAKE_BINARY_DIR}/bin/StatsApp.exe"
        DESTINATION bin
        COMPONENT statsapp_program)

# 1.2 Документация (MAN-страница в /usr/share/man/man1)
install(FILES "${CMAKE_CURRENT_SOURCE_DIR}/MainApp/statsapp.1"
        DESTINATION share/man/man1
        COMPONENT statsapp_program)

# 2. Компонент "Библиотека (основной)" (Двоичный файл в /usr/lib)
install(FILES "${CMAKE_BINARY_DIR}/lib/StatsLib.dll"
        DESTINATION lib
        COMPONENT statslib_main)

# 3. Компонент "Библиотека (разработчик)" (Заголовочный файл в /usr/include)
# Создаем фиктивный заголовочный файл метаданных для C# библиотеки
file(WRITE "${CMAKE_BINARY_DIR}/StatsLib.h" "// StatsLib C# Library Metadata Header\n")
install(FILES "${CMAKE_BINARY_DIR}/StatsLib.h"
        DESTINATION include
        COMPONENT statslib_dev)

# =========================================================================
# НАСТРОЙКА CPACK ДЛЯ ГЕНЕРАЦИИ DEB ПАКЕТОВ
# =========================================================================

# Включаем компонентную сборку пакетов
set(CPACK_GENERATOR "DEB")
set(CPACK_COMPONENTS_ALL statsapp_program statslib_main statslib_dev)
set(CPACK_DEB_COMPONENT_INSTALL ON) # Разделять компоненты на отдельные .deb файлы

# Общая информация о пакетах
set(CPACK_PACKAGE_NAME "stats-solution")
set(CPACK_PACKAGE_VERSION "1.0.0")
set(CPACK_PACKAGE_ARCHITECTURE "all") # Так как C# (.NET/Mono) является платформонезависимым байт-кодом
set(CPACK_PACKAGE_CONTACT "your_email@domain.com")
set(CPACK_PACKAGE_DESCRIPTION "Программный комплекс статистического анализа данных")

# --- Настройка зависимостей для пакета ПРОГРАММЫ ---
set(CPACK_DEBIAN_STATSAPP_PROGRAM_PACKAGE_NAME "statsapp-program")
# Учитываем зависимость от основного пакета библиотеки и системного окружения Mono
set(CPACK_DEBIAN_STATSAPP_PROGRAM_PACKAGE_DEPENDS "statslib-main (= 1.0.0), mono-runtime, mono-mcs")

# --- Настройка основного пакета БИБЛИОТЕКИ ---
set(CPACK_DEBIAN_STATSLIB_MAIN_PACKAGE_NAME "statslib-main")
set(CPACK_DEBIAN_STATSLIB_MAIN_PACKAGE_DEPENDS "mono-runtime")

# --- Настройка пакета РАЗРАБОТЧИКА ---
set(CPACK_DEBIAN_STATSLIB_DEV_PACKAGE_NAME "statslib-dev")
set(CPACK_DEBIAN_STATSLIB_DEV_PACKAGE_DEPENDS "statslib-main (= 1.0.0)")

include(CPack)
