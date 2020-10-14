import { btnHtml } from "./btn-html";

var closeCheck:number;
var win:Window;
var btn:Element;
var logo:Element;
var logoBack:Element;
const url:string = `http://172.26.17.60:8080/auth`; // Select a random ork

window.onload = function () {
  createButton();
};

function createButton(){
  btn = document.getElementById("tide");
  btn.innerHTML = btnHtml;

  logo = document.getElementById("tide-logo");
  logoBack = document.getElementById("logo-back");

  btn.addEventListener("click", function () {
    openAuth();
  });
}

function openAuth() {
  // Initialize
  win = window.open(url, "auth", "width=500, height=501,top=200,right=100");
  if (win == null) return;
  updateStatus("Awaiting login");
  toggleProcessing(true);

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

function updateStatus(msg:string) {
  document.getElementById("status-text").innerHTML = msg;
}

function handleCloseEarly() {
  updateStatus("Window closed without action");
  clearInterval(closeCheck);
  toggleProcessing(false);
}

function handleFinishAuthentication(data:any) {
  clearInterval(closeCheck);
  win.close();
  updateStatus("Finishing authentication");
  toggleProcessing(false);
  // Communicate with the vendor
}

function toggleProcessing(on:boolean){
  if(on){
    logo.classList.add("processing");
    logoBack.classList.add("processing");
  }else{
    logo.classList.remove("processing");
    logoBack.classList.remove("processing");
  }
}
