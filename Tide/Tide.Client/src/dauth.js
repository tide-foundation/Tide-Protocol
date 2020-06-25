import { DAuthFlow } from "tide";

const dev = true;
const nodes = dev ? 3 : 10;
const dauthUrl = "http://127.0.0.1:500";

function dauthFlow(user, nodes) {
  var urls = dev ? [...Array(nodes)].map((_, i) => dauthUrl + (i + 1)) : [...Array(nodes)].map((_, i) => `https://ork-${i}.azurewebsites.net`);
  return new DAuthFlow(urls, user);
}

async function signup() {
  var user = document.getElementById("register-username").value;
  var pass = document.getElementById("register-password").value;

  var flow = dauthFlow(user, nodes);
  var key = await flow.signUp(pass, user, nodes);

  console.log(`new authentication key for user ${user}:`, key.toString());
}

async function login() {
  var user = document.getElementById("login-username").value;
  var pass = document.getElementById("login-password").value;

  var flow = dauthFlow(user, nodes);
  var key = await flow.logIn(pass);

  console.log(`gathered authentication key for user ${user}:`, key.toString());
}

async function recover() {
  var user = document.getElementById("forget-username").value;
  var flow = dauthFlow(user, nodes);
  await flow.Recover();

  console.log('Please, Look for the shares in the email!!!');
}

async function reconstruct() {
  var user = document.getElementById("forget-username").value;
  var pass = document.getElementById("new-password").value;
  var shares = document.getElementById("shares").value;

  var flow = dauthFlow(user, nodes);
  var key = await flow.Reconstruct(shares, pass !== '' ? pass : null, nodes);

  console.log(`recovered authentication key for user ${user}:`, key.toString());
}

function main() {
  document.getElementById("login").addEventListener("click", login);
  document.getElementById("register").addEventListener("click", signup);
  document.getElementById("recover").addEventListener("click", recover);
  document.getElementById("reconstruct").addEventListener("click", reconstruct);
}

main();
