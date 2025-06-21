pluginManagement {
    repositories {
        google()
        mavenCentral()
        gradlePluginPortal()
    }
    plugins {
        id("com.android.application")        version "8.1.0"
        id("com.android.library")            version "8.1.0"
        id("org.jetbrains.kotlin.android")   version "1.9.10"
        id("androidx.navigation.safeargs.kotlin") version "2.5.3"
    }
}

dependencyResolutionManagement {
    repositoriesMode.set(RepositoriesMode.FAIL_ON_PROJECT_REPOS)
    repositories {
        google()
        mavenCentral()
    }
}

rootProject.name = "ProductManager"
include(":app")