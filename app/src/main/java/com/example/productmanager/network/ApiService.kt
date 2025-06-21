package com.example.productmanager.network

import okhttp3.MultipartBody
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.http.*

data class LoginRequest(val email: String, val password: String)
data class SignupRequest(val email: String, val password: String)
data class AuthResponse(val token: String)
data class InvoiceResponse(val missingFields: Boolean, val fields: List<String>)
data class ManualEntryRequest(val fields: Map<String, String>)
data class ItemDetails(val serial: String, val history: List<String>, val parts: List<String>)
data class PartInfo(val name: String, val frequency: Int)

interface ApiService {
    @POST("/api/auth/login")
    suspend fun login(@Body r: LoginRequest): Response<AuthResponse>

    @POST("/api/auth/signup")
    suspend fun signup(@Body r: SignupRequest): Response<AuthResponse>

    @Multipart
    @POST("/api/invoice/upload-invoice")
    suspend fun uploadInvoice(@Part f: MultipartBody.Part): Response<InvoiceResponse>

    @POST("/api/invoice/manual")
    suspend fun manualEntry(@Body r: ManualEntryRequest): Response<InvoiceResponse>

    @GET("/api/item/{serial}")
    suspend fun getItemData(@Path("serial") s: String): ItemDetails

    @GET("/api/item/{itemName}/popular-parts")
    suspend fun getPopularParts(@Path("itemName") n: String): List<PartInfo>

    companion object {
        fun create(): ApiService = Retrofit.Builder()
            .baseUrl("http://10.0.2.2:8080")
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(ApiService::class.java)
    }
}
