const dauthUrl = "http://127.0.0.1:500";
const nodes = 10;
const dev = false;

function dauthFlow(user, nodes) {
  var urls = dev ? [...Array(nodes)].map((_, i) => dauthUrl + (i + 1)) : [...Array(nodes)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
  return new cryptide.DAuthFlow(urls, user);
}

async function signup() {
  var user = document.getElementById("register-username").value;
  var pass = document.getElementById("register-password").value;

  var flow = dauthFlow(user, nodes);
  var key = await flow.signUp(pass, nodes);

  console.log(`new authentication key for user ${user}:`, key.toString());
}

async function login() {
  var user = document.getElementById("login-username").value;
  var pass = document.getElementById("login-password").value;

  var flow = dauthFlow(user, nodes);
  var key = await flow.logIn(pass);

  console.log(`gathered authentication key for user ${user}:`, key.toString());
}

async function main() {
  document.getElementById("login").addEventListener("click", login);
  document.getElementById("register").addEventListener("click", signup);
}

main();
