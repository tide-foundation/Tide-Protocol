var orkUrls = [...Array(3)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
var vendorUrl = "https://tidevendor.azurewebsites.net/";

// var orkUrls = [...Array(3)].map((_, i) => "http://localhost:500" + (i + 1));
// var vendorUrl = "http://127.0.0.1:6001";

var tide = new Tide("VendorId", vendorUrl, orkUrls, "publickey");

var user = "admin";
var pass = "123456";
var email = "tmp@tide.org";

console.log(tide);

tide
  .registerV2(user, pass, email, orkUrls)
  .then((registerResponse) => {
    console.log("Registration Successful");
    tide
      .loginV2(user, pass, orkUrls)
      .then((loginResponse) => {
        console.log("Login Successful");
      })
      .catch((loginError) => console.log(`Login Failed eith error: `, loginError));
  })
  .catch((registerError) => console.log(`Register Failed eith error: `, registerError));
