pluginManagement {
    repositories {
        google()
        mavenCentral()
        gradlePluginPortal()
    }
    plugins {
        id("com.android.application")        version "8.10.1"
        id("com.android.library")            version "8.10.1"
        id("org.jetbrains.kotlin.android")   version "1.9.21"
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