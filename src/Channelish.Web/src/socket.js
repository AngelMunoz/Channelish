import { webSocket } from "rxjs/webSocket";

export function connectTo(
  channel = "general",
  host = "ws://localhost:5000",
  token = sessionStorage.getItem("auth_token")
) {
  const url = encodeURI(`${host}/general?token=${token}`);
  return webSocket(url);
}
