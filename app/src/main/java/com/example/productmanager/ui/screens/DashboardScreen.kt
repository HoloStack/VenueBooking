package com.example.productmanager.ui.screens

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController

@Composable
fun DashboardScreen(navController: NavController) {
    Column(
        modifier = Modifier.fillMaxSize().padding(16.dp),
        verticalArrangement = Arrangement.Center,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Button(onClick = { navController.navigate("upload") }, Modifier.fillMaxWidth()) { Text("Upload Invoice") }
        Spacer(Modifier.height(8.dp))
        Button(onClick = { navController.navigate("view") }, Modifier.fillMaxWidth()) { Text("View Data") }
    }
}