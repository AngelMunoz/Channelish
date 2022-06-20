package me.tunaxor.apps.channelish_android

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.Button
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import me.tunaxor.apps.channelish_android.ui.theme.ChannelishAndroidTheme
import okhttp3.OkHttpClient
import okhttp3.WebSocket

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            ChannelishAndroidTheme {
                // A surface container using the 'background' color from the theme
                Surface(
                    modifier = Modifier.fillMaxSize(),
                    color = MaterialTheme.colorScheme.background
                ) {
                    Greeting("Android")
                }
            }
        }
    }
}

val client = OkHttpClient()
var ws: WebSocket? = null

@Composable
fun Greeting(name: String) {
    Text(text = "Hello $name!")
    Row() {
        Button(onClick = {
            ws?.close(1000, "Reconnecting")
            ws = getChannelishWebSocket("http://192.168.100.29:5000", "general", client)
        }) {
            Text("Connect To Server")
        }
        Button(onClick = { ws?.send("""{ "Ref": 1, "Topic": "status-change", "Payload": { "message": "olv" } }""") }) {
            Text("Send Message")
        }
    }
}

@Preview(showBackground = true)
@Composable
fun DefaultPreview() {
    ChannelishAndroidTheme {
        Greeting("Android")
    }
}