import { btnHtml } from "./btn-html";

var closeCheck: number;
var win: Window;
var btn: Element;
var logo: Element;
var logoBack: Element;
var chosenOrk: string;
var serverUrl: string;
var homeUrl: string;
var vendorPublic: string;

window.onload = () => createButton();

function createButton() {
  btn = document.getElementById("tide");
  btn.innerHTML = btnHtml;

  logo = document.getElementById("tide-logo");
  logoBack = document.getElementById("logo-back");

  btn.addEventListener("click", () => openAuth());
}

function openAuth() {
  // Initialize
  win = window.open(chosenOrk, homeUrl, "width=800, height=501,top=0,right=0"); // Using name as home url. This is a dirty way I found to feed in the return url initially
  if (win == null) return;
  updateStatus("Awaiting login");
  toggleProcessing(true);

  // Check for window close
  closeCheck = window.setInterval(() => {
    if (win.closed) handleCloseEarly();
  }, 100);

  // Listen for events from window
  window.addEventListener("message", (e) => {
    if (e.data.type == "tide-onload") win.postMessage({ type: "tide-init", serverUrl, vendorPublic }, chosenOrk);
    if (e.data.type == "tide-authenticated") handleFinishAuthentication(e.data);
  });
}

function updateStatus(msg: string) {
  document.getElementById("status-text").innerHTML = msg;
}

function handleCloseEarly() {
  updateStatus("Window closed without action");
  clearInterval(closeCheck);
  toggleProcessing(false);
}

function handleFinishAuthentication(data: any) {
  console.log(data.data.jwt);
  clearInterval(closeCheck);
  updateStatus("Finishing authentication");

  // Communicate with the vendor

  win.close();
  toggleProcessing(false);
  updateStatus(`jwt: ${data.data.jwt}`);
}

function toggleProcessing(on: boolean) {
  logo.classList[on ? "add" : "remove"]("processing");
  logoBack.classList[on ? "add" : "remove"]("processing");
}

export function init(_homeUrl: string, _serverUrl: string, _chosenOrk: string, _vendorPublic: string) {
  homeUrl = _homeUrl;
  serverUrl = _serverUrl;
  chosenOrk = _chosenOrk;
  vendorPublic = _vendorPublic;
}