cmake_minimum_required(VERSION 3.10)
project(haptic_library)

set(CMAKE_CXX_STANDARD 11)

IF (WIN32)
ELSE()
    set(THREADS_PREFER_PTHREAD_FLAG ON)
    find_package(Threads REQUIRED)
ENDIF()

add_library(haptic_library SHARED
        src/HapticLibrary.cpp
        src/hapticsManager.cpp src/hapticsManager.h
        src/util.cpp src/util.h
        src/timer.cpp src/timer.h
        src/ProjectModel.cpp src/ProjectModel.h
        src/easywsclient.cpp src/easywsclient.h
        src/commonutils.h
        src/commonutils.cpp)

target_compile_definitions(haptic_library PRIVATE BHAPTICS_BUILDING_DLL)

IF (WIN32)
    SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /UMBCS /D_UNICODE /DUNICODE")
ELSE()
    target_link_libraries(haptic_library Threads::Threads)
ENDIF()

add_library(hello::library ALIAS haptic_library)


target_include_directories(haptic_library
        PUBLIC
        ${PROJECT_SOURCE_DIR}/include
        )

add_executable(hello_binary main.cpp)

target_link_libraries( hello_binary
        PRIVATE
        hello::library
        )

