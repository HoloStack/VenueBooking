package com.example.productmanager.ui.screens
import android.net.Uri
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController
import coil.compose.rememberAsyncImagePainter
import com.example.productmanager.network.ApiService
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch

import okhttp3.MediaType.Companion.toMediaType
import okhttp3.MultipartBody
import okhttp3.RequestBody.Companion.asRequestBody
import java.io.File

@Composable
fun UploadInvoiceScreen(navController: NavController) {
    var uri by remember { mutableStateOf<Uri?>(null) }
    val picker = rememberLauncherForActivityResult(ActivityResultContracts.GetContent()) { uri = it }
    Column(Modifier.fillMaxSize().padding(16.dp)) {
        Button(onClick = { picker.launch("image/*") }, Modifier.fillMaxWidth()) { Text("Select Photo") }
        uri?.let { selected ->
            Image(painter = rememberAsyncImagePainter(selected), contentDescription = null, Modifier.fillMaxWidth().height(200.dp))
            Spacer(Modifier.height(8.dp))
            Button(onClick = {
                val file = File(selected.path!!)
                val body = file.asRequestBody("image/*".toMediaType())
                val part = MultipartBody.Part.createFormData("file", file.name, body)
                CoroutineScope(Dispatchers.IO).launch {
                    val resp = ApiService.create().uploadInvoice(part)
                    val missing = resp.body()?.missingFields == true
                    val fields = resp.body()?.fields?.joinToString(",")
                    navController.navigate(if (missing && fields!=null) "manual/$fields" else "dashboard")
                }
            }, Modifier.fillMaxWidth()) { Text("Upload") }
        }
    }
}