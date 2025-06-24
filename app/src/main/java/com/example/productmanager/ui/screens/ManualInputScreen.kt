package com.example.productmanager.ui.screens
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController
import com.example.productmanager.network.ApiService
import com.example.productmanager.network.ManualEntryRequest
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch

@Composable
fun ManualInputScreen(navController: NavController, fieldsCsv: String?) {
    val fields = fieldsCsv?.split(",") ?: emptyList()
    val inputs = remember { mutableStateMapOf<String, String>() }
    Column(Modifier.fillMaxSize().padding(16.dp)) {
        fields.forEach { field ->
            var text by remember { mutableStateOf("") }
            OutlinedTextField(text, { text = it; inputs[field] = it }, label={ Text(field) })
            Spacer(Modifier.height(8.dp))
        }
        Button(onClick={
            CoroutineScope(Dispatchers.IO).launch {
                ApiService.create().manualEntry(ManualEntryRequest(inputs))
                navController.navigate("dashboard")
            }
        }, Modifier.fillMaxWidth()) { Text("Submit") }
    }
}