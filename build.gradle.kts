// Top-level build file where you can add configuration options common to all sub-projects/modules.
//plugins {
//    // Apply plugin aliases from version catalog
//    alias(libs.plugins.android.application) apply false
//    alias(libs.plugins.kotlin.android) apply false
//    alias(libs.plugins.kotlin.compose) apply false
//}
plugins {
    // Android & Kotlin
    id("com.android.application")  version "8.1.0" apply false
    id("com.android.library")      version "8.1.0" apply false
    id("org.jetbrains.kotlin.android") version "1.9.20" apply false

    // Navigation Safe Args (optional)
    id("androidx.navigation.safeargs.kotlin") version "2.5.3" apply false

    // ← Add Google Services here:
    id("com.google.gms.google-services")     version "4.3.15" apply false
}

tasks.register<Delete>("clean") {
    delete(rootProject.buildDir)
}