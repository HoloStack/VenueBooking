package com.example.productmanager.ui.screens

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController
import com.example.productmanager.network.ApiService
import com.example.productmanager.network.ItemDetails
import com.example.productmanager.network.PartInfo
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch

@Composable
fun ViewDataScreen(navController: NavController) {
    var serial by remember { mutableStateOf("") }
    var itemName by remember { mutableStateOf("") }
    var details by remember { mutableStateOf<ItemDetails?>(null) }
    var popular by remember { mutableStateOf<List<PartInfo>>(emptyList()) }
    Column(Modifier.fillMaxSize().padding(16.dp)) {
        OutlinedTextField(serial, { serial = it }, label={ Text("Serial") })
        Spacer(Modifier.height(8.dp))
        Button(onClick={ CoroutineScope(Dispatchers.IO).launch { details = ApiService.create().getItemData(serial) } }, Modifier.fillMaxWidth()) { Text("Lookup") }
        details?.let { Text("History: ${it.history}\nParts: ${it.parts}") }
        Spacer(Modifier.height(16.dp))
        OutlinedTextField(itemName, { itemName = it }, label={ Text("Item Name") })
        Spacer(Modifier.height(8.dp))
        Button(onClick={ CoroutineScope(Dispatchers.IO).launch { popular = ApiService.create().getPopularParts(itemName) } }, Modifier.fillMaxWidth()) { Text("Popular Parts") }
        popular.forEach { Text("${it.name}: ${it.frequency}") }
    }
}