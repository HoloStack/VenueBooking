package com.yourcompany.productmanager.ui.theme

import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.graphics.Color
import com.example.productmanager.ui.theme.Shapes
import com.example.productmanager.ui.theme.Typography

// 1) Define your color schemes
private val DarkColors = darkColorScheme(
    primary = Color(0xFF1EB980),
    secondary = Color(0xFF045D56),
    background = Color(0xFF26282F),
    surface = Color(0xFF1E1F28),
    onPrimary = Color.White,
    onSecondary = Color.White,
    onBackground = Color.White,
    onSurface = Color.White
)

private val LightColors = lightColorScheme(
    primary = Color(0xFF006E51),
    secondary = Color(0xFF018786),
    background = Color(0xFFF5F5F5),
    surface = Color.White,
    onPrimary = Color.White,
    onSecondary = Color.White,
    onBackground = Color.Black,
    onSurface = Color.Black
)

// 2) Wrap MaterialTheme in your own @Composable
@Composable
fun ProductManagerTheme(
    darkTheme: Boolean = isSystemInDarkTheme(),      // default to system setting
    content: @Composable () -> Unit                  // **note** the type here!
) {
    val colors = if (darkTheme) DarkColors else LightColors

    MaterialTheme(
        colorScheme = colors,
        typography = Typography,   // from your Typography.kt
        shapes = Shapes,           // from your Shapes.kt
        content = content
    )
}