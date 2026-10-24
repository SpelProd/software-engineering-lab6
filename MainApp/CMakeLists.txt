# ВЫПОЛНЕНИЕ ТЗ: Функция проверки наличия библиотеки в проекте
if(NOT TARGET StatsLib)
    message(FATAL_ERROR "Критическая ошибка CMake: Библиотека 'StatsLib' не найдена! Сборка невозможна.")
else()
    message(STATUS "Проверка CMake: Библиотека 'StatsLib' успешно найдена. Начинаем конфигурацию приложения.")
endif()

# Создаем кастомную цель для сборки StatsApp.exe
add_custom_target(StatsApp ALL
    COMMAND csc /target:exe /out:StatsApp.exe /reference:${CMAKE_BINARY_DIR}/lib/StatsLib.dll Program.cs
    WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}
    SOURCES Program.cs
)

# Говорим CMake, что StatsApp зависит от StatsLib (сначала соберется библиотека)
add_dependencies(StatsApp StatsLib)

# Собираем всё в одну финальную папку bin
add_custom_command(TARGET StatsApp POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E make_directory ${CMAKE_BINARY_DIR}/bin
    COMMAND ${CMAKE_COMMAND} -E copy ${CMAKE_CURRENT_SOURCE_DIR}/StatsApp.exe ${CMAKE_BINARY_DIR}/bin/
    COMMAND ${CMAKE_COMMAND} -E copy ${CMAKE_BINARY_DIR}/lib/StatsLib.dll ${CMAKE_BINARY_DIR}/bin/
)

# =========================================================================
# ИНТЕГРАЦИЯ И УСТАНОВКА MAN-СТРАНИЦЫ И ПРИЛОЖЕНИЯ (ЛР 1.2 / ЛР 1.3)
# =========================================================================

# Определяем стандартный путь для установки man-страниц в Unix/Linux
# Обычно это CMAKE_INSTALL_PREFIX/share/man/man1 (например, /usr/local/share/man/man1)
set(MAN_INSTALL_DIR "share/man/man1")

# Правило установки исполняемого файла программы
install(FILES "${CMAKE_CURRENT_SOURCE_DIR}/StatsApp.exe"
        DESTINATION bin)

# Правило установки сопутствующей библиотеки рядом с исполняемым файлом
install(FILES "${CMAKE_BINARY_DIR}/lib/StatsLib.dll"
        DESTINATION bin)

# Правило установки man-страницы в системную директорию
install(FILES "${CMAKE_CURRENT_SOURCE_DIR}/statsapp.1"
        DESTINATION "${MAN_INSTALL_DIR}")
