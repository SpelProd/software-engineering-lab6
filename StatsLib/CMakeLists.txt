# Создаем кастомную цель для сборки StatsLib.dll напрямую через csc
add_custom_target(StatsLib ALL
    COMMAND csc /target:library /out:StatsLib.dll /doc:StatsLib.xml StatsCalculator.cs
    WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}
    SOURCES StatsCalculator.cs
)

# Копируем результаты в общую папку сборки, чтобы приложение могло их найти
add_custom_command(TARGET StatsLib POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E make_directory ${CMAKE_BINARY_DIR}/lib
    COMMAND ${CMAKE_COMMAND} -E copy ${CMAKE_CURRENT_SOURCE_DIR}/StatsLib.dll ${CMAKE_BINARY_DIR}/lib/
    COMMAND ${CMAKE_COMMAND} -E copy ${CMAKE_CURRENT_SOURCE_DIR}/StatsLib.xml ${CMAKE_BINARY_DIR}/lib/
)
