package com.example.productmanager

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import com.example.productmanager.ui.theme.ProductManagerTheme
import com.example.productmanager.navigation.AppNavHost

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            ProductManagerTheme() { AppNavHost() }
        }
    }
}