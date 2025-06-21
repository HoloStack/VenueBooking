package com.yourcompany.productmanager

import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.result.contract.ActivityResultContracts
import com.google.android.gms.auth.api.signin.GoogleSignIn
import com.google.android.gms.auth.api.signin.GoogleSignInClient
import com.google.android.gms.auth.api.signin.GoogleSignInOptions
import com.google.android.gms.common.api.ApiException
import com.yourcompany.productmanager.ui.SignInScreen

class MainActivity : ComponentActivity() {
    private lateinit var googleSignInClient: GoogleSignInClient

    // Register for activity result using Activity Result API
    private val signInLauncher = registerForActivityResult(
        ActivityResultContracts.StartActivityForResult()
    ) { result ->
        val data: Intent? = result.data
        if (result.resultCode == RESULT_OK && data != null) {
            val task = GoogleSignIn.getSignedInAccountFromIntent(data)
            try {
                val account = task.getResult(ApiException::class.java)!!
                // ID token available via account.idToken
                // TODO: sendIdTokenToYourBackend(account.idToken!!)
                Toast.makeText(this, "Signed in as ${account.email}", Toast.LENGTH_LONG).show()
            } catch (e: ApiException) {
                Toast.makeText(this, "Sign-in failed: ${e.statusCode}", Toast.LENGTH_LONG).show()
            }
        }
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        // Initialize Google Sign-In options
        val gso = GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_SIGN_IN)
            .requestIdToken(getString(R.string.server_client_id))
            .requestEmail()
            .build()

        googleSignInClient = GoogleSignIn.getClient(this, gso)

        // If using Jetpack Compose:
        setContent {
            SignInScreen(
                onSignInClicked = { launchSignIn() }
            )
        }

        // If using XML layout instead:
        // setContentView(R.layout.activity_main)
        // findViewById<Button>(R.id.sign_in_button)
        //     .setOnClickListener { launchSignIn() }
    }

    private fun launchSignIn() {
        val signInIntent = googleSignInClient.signInIntent
        signInLauncher.launch(signInIntent)
    }
}