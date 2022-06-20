package me.tunaxor.apps.channelish_android

import okhttp3.*

val rooms = mutableMapOf<String, WebSocket>()

class ChannelishWebSocket (private val room: String): WebSocketListener() {
    override fun onOpen(webSocket: WebSocket, response: Response) {
        super.onOpen(webSocket, response)
        if (rooms.contains(room)) { return }
        rooms[room] = webSocket;
        println("[$room]: Connected")
    }
    override fun onMessage(webSocket: WebSocket, text: String) {
        super.onMessage(webSocket, text)
        println("[$room]: Sent Message $text")
    }
    override fun onClosing(webSocket: WebSocket, code: Int, reason: String) {
        super.onClosing(webSocket, code, reason)
        println("[$room]: Disconnecting - $code:$reason")
    }

    override fun onClosed(webSocket: WebSocket, code: Int, reason: String) {
        super.onClosed(webSocket, code, reason)
        rooms.remove(room)
        println("[$room]: Disconnected - $code:$reason")
    }

    override fun onFailure(webSocket: WebSocket, t: Throwable, response: Response?) {
        super.onFailure(webSocket, t, response)
        println("[$room]: Failed - ${t.message} - ${response?.code}:${response?.message}")
    }
}


fun getChannelishWebSocket(host: String, room: String, client: OkHttpClient): WebSocket {
    val request: Request = Request.Builder().url("$host/$room").build()
    val listener = ChannelishWebSocket(room)
    return client.newWebSocket(request, listener)
}
