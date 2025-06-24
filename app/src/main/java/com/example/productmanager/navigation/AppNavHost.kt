package com.example.productmanager.navigation

import androidx.compose.runtime.Composable
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.example.productmanager.ui.screens.*

@Composable
fun AppNavHost() {
    val navController = rememberNavController()
    NavHost(navController, startDestination = "login") {
        composable("login") { LoginScreen(navController) }
        composable("signup") { SignupScreen(navController) }
        composable("dashboard") { DashboardScreen(navController) }
        composable("upload") { UploadInvoiceScreen(navController) }
        composable(
            "manual/{fields}",
            arguments = listOf(navArgument("fields") { type = NavType.StringType })
        ) { backStackEntry ->
            ManualInputScreen(navController, backStackEntry.arguments?.getString("fields"))
        }
        composable("view") { ViewDataScreen(navController) }
    }
}
