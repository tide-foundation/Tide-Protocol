var user = "admin";
var pass = "123456";
var email = "tmp@tide.org";

var localBtn = document.getElementById("local");
var remoteBtn = document.getElementById("remote");
var runBtn = document.getElementById("run");
var logDiv = document.getElementById("log");

var orkUrls = [...Array(3)].map((_, i) => "http://localhost:500" + (i + 1));
var vendorUrl = "http://127.0.0.1:6001";

localBtn.onclick = function () {
  orkUrls = [...Array(3)].map((_, i) => "http://localhost:500" + (i + 1));
  vendorUrl = "http://127.0.0.1:6001";
};

remoteBtn.onclick = function () {
  orkUrls = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
  vendorUrl = "https://tidevendor.azurewebsites.net/";
};

runBtn.onclick = function () {
  var tide = new Tide("VendorId", vendorUrl, orkUrls, "publickey");

  tide
    .registerV2(user, pass, email, orkUrls)
    .then((registerResponse) => {
      log("Registration Successful");
      tide
        .loginV2(user, pass, orkUrls)
        .then((loginResponse) => {
          log("Login Successful");
        })
        .catch((loginError) => log(`Login Failed`, loginError));
    })
    .catch((registerError) => log(`Register Failed`, registerError));
};

function log(msg, error) {
  error != null ? console.log(msg, error) : console.log(msg);
  logDiv.innerHTML += `<p class="${error != null ? "error" : "success"}">${msg}</p>`;
}
