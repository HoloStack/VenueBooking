package com.example.productmanager.ui.theme

import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.ui.graphics.Color

private val LightColors = lightColorScheme(primary=Color(0xFF006A4E), secondary=Color(0xFF4CAF50))

@Composable
fun ProductManagerTheme(content:@Composable()->Unit) {
    MaterialTheme(colorScheme=LightColors, typography=Typography, content=content)
}