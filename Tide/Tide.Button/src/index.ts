import { Config } from "./models/Config";
import { btnHtml } from "./btn-html";

var closeCheck: number;
var win: Window;
var btn: Element;
var logo: Element;
var logoBack: Element;

var config: Config;

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
  win = window.open(config.chosenOrk, config.homeUrl, "width=550, height=650,top=0,right=0"); // Using name as home url. This is a dirty way I found to feed in the return url initially
  if (win == null) return;
  updateStatus("Awaiting login");
  toggleProcessing(true);

  // Check for window close
  closeCheck = window.setInterval(() => {
    if (win.closed) handleCloseEarly();
  }, 100);

  // Listen for events from window
  window.addEventListener("message", (e) => {
    if (e.data.type == "tide-onload") win.postMessage({ type: "tide-init", serverUrl: config.serverUrl, vendorPublic: config.vendorPublic, hashedReturnUrl: config.hashedReturnUrl }, config.chosenOrk);
    if (e.data.type == "tide-authenticated") handleFinishAuthentication(e.data);
    if (e.data.type == "tide-failed") handleTideFailed(e.data);
    if (e.data.type == "tide-change-ork") handleChangeOrk(e.data);
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
  clearInterval(closeCheck);
  updateStatus("Finishing authentication");

  if (data.data.autoClose) win.close();
  toggleProcessing(false);
  win = null;
  window.dispatchEvent(new CustomEvent("tide-auth", { detail: data }));

  updateStatus("Complete");
}

function handleTideFailed(data: any) {
  clearInterval(closeCheck);
  win.close();
  updateStatus(data.data.error);
}

function handleChangeOrk(data: any) {
  clearInterval(closeCheck);
  win.close();
  config.chosenOrk = data.newOrk;
  openAuth();
}

function toggleProcessing(on: boolean) {
  logo.classList[on ? "add" : "remove"]("processing");
  logoBack.classList[on ? "add" : "remove"]("processing");
}

export function init(configuration: Config) {
  config = configuration;
}
