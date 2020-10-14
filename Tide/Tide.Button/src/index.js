import { btnHtml } from "./btn-html";

var closeCheck;
var win;
const url = `http://172.26.17.60:8080/auth`; // Select a random ork

window.onload = function () {
  const btn = document.getElementById("tide");
  btn.innerHTML = btnHtml;

  btn.addEventListener("click", function () {
    openAuth();
  });
};

function openAuth() {
  // Initialize
  win = window.open(url, "auth", "width=500, height=501");
  if (win == null) return;
  updateStatus("Awaiting login");

  // Check for window close
  closeCheck = window.setInterval(() => {
    if (win.closed) handleCloseEarly();
  }, 100);

  // Listen for events from window
  window.addEventListener(
    "message",
    (e) => {
      const data = JSON.parse(e.data);
      if (data.type == "authenticated") handleFinishAuthentication(data);
    },
    false
  );
}

function updateStatus(msg) {
  document.getElementById("tide-loader").style.display = "block";
  document.getElementById("status-text").innerHTML = msg;
}

function handleCloseEarly() {
  updateStatus("Window closed without action");
  clearInterval(closeCheck);
}

function handleFinishAuthentication(data) {
  clearInterval(closeCheck);
  win.close();
  updateStatus("Finishing authentication");

  // Communicate with the vendor
}
