plugins {
    id("com.android.application")         version "8.10.1" apply false
    id("com.android.library")             version "8.10.1" apply false
    id("org.jetbrains.kotlin.android")    version "1.9.21" apply false
    id("androidx.navigation.safeargs.kotlin") version "2.5.3" apply false
}

tasks.register<Delete>("clean") {
    delete(rootProject.buildDir)
}