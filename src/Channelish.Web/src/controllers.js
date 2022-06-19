/**
 * @type {Map<string, WebSocket>}
 */
const rooms = new Map();
const host = "ws://localhost:5000";

export class SocketController {
  constructor(host) {
    this.host = host;
    host.addController(this);
    this.atLeastOne = false;
  }

  connectTo(channel = "general") {
    if (rooms.has(channel)) return;
    const socket = new WebSocket(`${host}/${channel}`);
    socket.addEventListener(
      "open",
      /**
       * @type {WebSocketEventMap['open']} event
       */
      (event) => {
        this.onChannelOpen(channel, socket, event);
      }
    );
    socket.addEventListener(
      "message",
      /**
       * @type {WebSocketEventMap['message']} event
       */
      (event) => {
        this.onChannelMessage(channel, socket, event);
      }
    );
    socket.addEventListener(
      "error",
      /**
       * @type {WebSocketEventMap['error']} event
       */
      (event) => {
        this.onChannelError(channel, socket, event);
      }
    );
    socket.addEventListener(
      "close",
      /**
       * @type {WebSocketEventMap['close']} event
       */
      (event) => {
        this.onChannelClose(channel, socket, event);
      }
    );
  }

  disconnectFrom(channel) {
    const socket = rooms.get(channel);
    socket?.close();
  }

  /**
   *
   * @param {string} channel
   * @param {string} topic
   * @param {any} payload
   */
  sendMessageTo(channel, topic, payload) {
    const socket = rooms.get(channel);
    const data = { topic, ref: 1, payload };
    socket?.send(JSON.stringify(data));
  }

  /**
   *
   * @param {string} channel
   * @param {WebSocket} socket
   * @param {WebSocketEventMap['open']} event
   */
  onChannelOpen(channel, socket, event) {
    rooms.set(channel, socket);
    console.log(`[${channel}]: Connected`);
    this.atLeastOne = true;
    this.host.requestUpdate();
  }

  /**
   *
   * @param {string} channel
   * @param {WebSocket} socket
   * @param {WebSocketEventMap['message']} event
   */
  onChannelMessage(channel, socket, event) {
    console.log(`[${channel}]: Sent Message`, event.data);
  }
  /**
   *
   * @param {string} channel
   * @param {WebSocket} socket
   * @param {WebSocketEventMap['error']} event
   */
  onChannelError(channel, socket, event) {
    console.error(`[${channel}]: Error`);
    if (
      socket.readyState === socket.CLOSED ||
      socket.readyState === socket.CLOSING
    ) {
      console.warn(`[Channel ${channel}]: Closing`);
      rooms.delete(channel);
    }
  }
  /**
   *
   * @param {string} channel
   * @param {WebSocket} socket
   * @param {WebSocketEventMap['close']} event
   */
  onChannelClose(channel, socket, event) {
    console.error(`[${channel}]: Closed with code ${event.code}`);
    rooms.delete(channel);
    if (rooms.size <= 0) {
      this.atLeastOne = false;
      this.host.requestUpdate();
    }
  }
}
