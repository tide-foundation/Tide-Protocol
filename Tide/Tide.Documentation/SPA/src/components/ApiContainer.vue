<template>
  <div v-html="content"></div>
</template>

<script>
export default {
  data: function() {
    return {
      content: `<div id="main">
      <h1 class="page-title">Class: Tide</h1>

      <section>
        <header>
          <h2>
            <span class="attribs"><span class="type-signature"></span></span>Tide<span class="signature">(vendorId, serverUrl)</span><span class="type-signature"></span>
          </h2>

          <div class="class-description">The client side library to interface with the Tide ecosystem.</div>
        </header>

        <article>
          <div class="container-overview">
            <h2>Constructor</h2>

            <h4 class="name" id="Tide"><span class="type-signature"></span>new Tide<span class="signature">(vendorId, serverUrl)</span><span class="type-signature"></span></h4>

            <div class="description">
              Initialize Tide.
            </div>

            <h5>Parameters:</h5>

            <table class="params">
              <thead>
                <tr>
                  <th>Name</th>

                  <th>Type</th>

                  <th class="last">Description</th>
                </tr>
              </thead>

              <tbody>
                <tr>
                  <td class="name"><code>vendorId</code></td>

                  <td class="type">
                    <span class="param-type">String</span>
                  </td>

                  <td class="description last">Your designated VendorId in which you will operate</td>
                </tr>

                <tr>
                  <td class="name"><code>serverUrl</code></td>

                  <td class="type">
                    <span class="param-type">String</span>
                  </td>

                  <td class="description last">The endpoint of your backend Tide server</td>
                </tr>
              </tbody>
            </table>

            <dl class="details">
              <dt class="tag-source">Source:</dt>
              <dd class="tag-source">
                <ul class="dummy">
                  <li><a href="Tide.js.html">Tide.js</a>, <a href="Tide.js.html#line20">line 20</a></li>
                </ul>
              </dd>
            </dl>
          </div>

          <h3 class="subsection-title">Methods</h3>

          <h4 class="name" id="decrypt"><span class="type-signature"></span>decrypt<span class="signature">(cipher)</span><span class="type-signature"> &rarr; {String}</span></h4>

          <div class="description">
            Decrypt an encrypted string with the logged in user keys. This action requires a logged in user.
          </div>

          <h5>Parameters:</h5>

          <table class="params">
            <thead>
              <tr>
                <th>Name</th>

                <th>Type</th>

                <th class="last">Description</th>
              </tr>
            </thead>

            <tbody>
              <tr>
                <td class="name"><code>cipher</code></td>

                <td class="type">
                  <span class="param-type">String</span>
                </td>

                <td class="description last">The encrypted string you wish to decrypt using the user keys</td>
              </tr>
            </tbody>
          </table>

          <dl class="details">
            <dt class="tag-source">Source:</dt>
            <dd class="tag-source">
              <ul class="dummy">
                <li><a href="Tide.js.html">Tide.js</a>, <a href="Tide.js.html#line130">line 130</a></li>
              </ul>
            </dd>
          </dl>

          <h5>Returns:</h5>

          <div class="param-desc">
            - The plain text message
          </div>

          <dl>
            <dt>
              Type
            </dt>
            <dd>
              <span class="param-type">String</span>
            </dd>
          </dl>

          <h4 class="name" id="encrypt"><span class="type-signature"></span>encrypt<span class="signature">(msg)</span><span class="type-signature"> &rarr; {String}</span></h4>

          <div class="description">
            Encrypt a string with the logged in user keys. This action requires a logged in user.
          </div>

          <h5>Parameters:</h5>

          <table class="params">
            <thead>
              <tr>
                <th>Name</th>

                <th>Type</th>

                <th class="last">Description</th>
              </tr>
            </thead>

            <tbody>
              <tr>
                <td class="name"><code>msg</code></td>

                <td class="type">
                  <span class="param-type">String</span>
                </td>

                <td class="description last">The string you wish to encrypt using the user keys</td>
              </tr>
            </tbody>
          </table>

          <dl class="details">
            <dt class="tag-source">Source:</dt>
            <dd class="tag-source">
              <ul class="dummy">
                <li><a href="Tide.js.html">Tide.js</a>, <a href="Tide.js.html#line116">line 116</a></li>
              </ul>
            </dd>
          </dl>

          <h5>Returns:</h5>

          <div class="param-desc">
            - The encrypted payload
          </div>

          <dl>
            <dt>
              Type
            </dt>
            <dd>
              <span class="param-type">String</span>
            </dd>
          </dl>

          <h4 class="name" id="login"><span class="type-signature"></span>login<span class="signature">(username, password)</span><span class="type-signature"> &rarr; {AESKey}</span></h4>

          <div class="description">
            Login to a previously created Tide account. The account must be fully enabled by the vendor before use. This will generate a new Tide user using the provided username and providing a keypaid to manage the account (user-secret).
          </div>

          <h5>Parameters:</h5>

          <table class="params">
            <thead>
              <tr>
                <th>Name</th>

                <th>Type</th>

                <th class="last">Description</th>
              </tr>
            </thead>

            <tbody>
              <tr>
                <td class="name"><code>username</code></td>

                <td class="type">
                  <span class="param-type">String</span>
                </td>

                <td class="description last">Plain text username of the user</td>
              </tr>

              <tr>
                <td class="name"><code>password</code></td>

                <td class="type">
                  <span class="param-type">String</span>
                </td>

                <td class="description last">Plain text password of the user</td>
              </tr>
            </tbody>
          </table>

          <dl class="details">
            <dt class="tag-source">Source:</dt>
            <dd class="tag-source">
              <ul class="dummy">
                <li><a href="Tide.js.html">Tide.js</a>, <a href="Tide.js.html#line80">line 80</a></li>
              </ul>
            </dd>
          </dl>

          <h5>Returns:</h5>

          <div class="param-desc">
            - The users keys to be used on the data
          </div>

          <dl>
            <dt>
              Type
            </dt>
            <dd>
              <span class="param-type">AESKey</span>
            </dd>
          </dl>

          <h4 class="name" id="logout"><span class="type-signature"></span>logout<span class="signature">()</span><span class="type-signature"></span></h4>

          <div class="description">
            Strips all local user data from the browser.
          </div>

          <dl class="details">
            <dt class="tag-source">Source:</dt>
            <dd class="tag-source">
              <ul class="dummy">
                <li><a href="Tide.js.html">Tide.js</a>, <a href="Tide.js.html#line103">line 103</a></li>
              </ul>
            </dd>
          </dl>

          <h4 class="name" id="reconstruct"><span class="type-signature"></span>reconstruct<span class="signature">(username, shares, newPass)</span><span class="type-signature"></span></h4>

          <div class="description">
            Login to a previously created Tide account. The account must be fully enabled by the vendor before use. This will generate a new Tide user using the provided username and providing a keypaid to manage the account (user-secret).
          </div>

          <h5>Parameters:</h5>

          <table class="params">
            <thead>
              <tr>
                <th>Name</th>

                <th>Type</th>

                <th class="last">Description</th>
              </tr>
            </thead>

            <tbody>
              <tr>
                <td class="name"><code>username</code></td>

                <td class="type">
                  <span class="param-type">String</span>
                </td>

                <td class="description last">Plain text username of the user</td>
              </tr>

              <tr>
                <td class="name"><code>shares</code></td>

                <td class="type">
                  <span class="param-type">Array</span>
                </td>

                <td class="description last">An array of shares sent to the users email(s)</td>
              </tr>

              <tr>
                <td class="name"><code>newPass</code></td>

                <td class="type">
                  <span class="param-type">String</span>
                </td>

                <td class="description last">The new password of the user</td>
              </tr>
            </tbody>
          </table>

          <dl class="details">
            <dt class="tag-source">Source:</dt>
            <dd class="tag-source">
              <ul class="dummy">
                <li><a href="Tide.js.html">Tide.js</a>, <a href="Tide.js.html#line162">line 162</a></li>
              </ul>
            </dd>
          </dl>

          <h4 class="name" id="recover"><span class="type-signature">(async) </span>recover<span class="signature">(username)</span><span class="type-signature"></span></h4>

          <div class="description">
            Send a request to the ORK nodes used by the user to email them recovery shards. This is step 1 in a 2 step process to recover the user keys.
          </div>

          <h5>Parameters:</h5>

          <table class="params">
            <thead>
              <tr>
                <th>Name</th>

                <th>Type</th>

                <th class="last">Description</th>
              </tr>
            </thead>

            <tbody>
              <tr>
                <td class="name"><code>username</code></td>

                <td class="type">
                  <span class="param-type">String</span>
                </td>

                <td class="description last">The username of the user who wishes to recover</td>
              </tr>
            </tbody>
          </table>

          <dl class="details">
            <dt class="tag-source">Source:</dt>
            <dd class="tag-source">
              <ul class="dummy">
                <li><a href="Tide.js.html">Tide.js</a>, <a href="Tide.js.html#line140">line 140</a></li>
              </ul>
            </dd>
          </dl>

          <h4 class="name" id="register"><span class="type-signature"></span>register<span class="signature">(username, password, email, orkIds)</span><span class="type-signature"> &rarr; {AESKey}</span></h4>

          <div class="description">
            Create a new Tide account. This will generate a new Tide user using the provided username and providing a keypaid to manage the account (user-secret).
          </div>

          <h5>Parameters:</h5>

          <table class="params">
            <thead>
              <tr>
                <th>Name</th>

                <th>Type</th>

                <th class="last">Description</th>
              </tr>
            </thead>

            <tbody>
              <tr>
                <td class="name"><code>username</code></td>

                <td class="type">
                  <span class="param-type">String</span>
                </td>

                <td class="description last">Plain text username of the new user</td>
              </tr>

              <tr>
                <td class="name"><code>password</code></td>

                <td class="type">
                  <span class="param-type">String</span>
                </td>

                <td class="description last">Plain text password of the new user</td>
              </tr>

              <tr>
                <td class="name"><code>email</code></td>

                <td class="type">
                  <span class="param-type">String</span>
                </td>

                <td class="description last">The recovery email to be used by the user.</td>
              </tr>

              <tr>
                <td class="name"><code>orkIds</code></td>

                <td class="type">
                  <span class="param-type">Array</span>
                </td>

                <td class="description last">The desired ork nodes to be used for registration. An account can only be activated when all ork nodes have confirmed they have stored the shard.</td>
              </tr>
            </tbody>
          </table>

          <dl class="details">
            <dt class="tag-source">Source:</dt>
            <dd class="tag-source">
              <ul class="dummy">
                <li><a href="Tide.js.html">Tide.js</a>, <a href="Tide.js.html#line39">line 39</a></li>
              </ul>
            </dd>
          </dl>

          <h5>Fires:</h5>
          <ul>
            <li>event:progress</li>
          </ul>

          <h5>Returns:</h5>

          <div class="param-desc">
            - The users keys to be used on the data
          </div>

          <dl>
            <dt>
              Type
            </dt>
            <dd>
              <span class="param-type">AESKey</span>
            </dd>
          </dl>
        </article>
      </section>
    </div>`,
    };
  },
  created() {},
};
</script>

<style scoped lang="scss">
html {
  overflow: auto;
  background-color: #fff;
  font-size: 14px;
}

body {
  font-family: "Open Sans", sans-serif;
  line-height: 1.5;
  color: #4d4e53;
  background-color: white;
}

a,
a:visited,
a:active {
  color: #0095dd;
  text-decoration: none;
}

a:hover {
  text-decoration: underline;
}

header {
  display: block;
  padding: 0px 4px;
}

tt,
code,
kbd,
samp {
  font-family: Consolas, Monaco, "Andale Mono", monospace;
}

.class-description {
  font-size: 130%;
  line-height: 140%;
  margin-bottom: 1em;
  margin-top: 1em;
}

.class-description:empty {
  margin: 0;
}

#main {
  float: left;
  width: 70%;
}

article dl {
  margin-bottom: 40px;
}

article img {
  max-width: 100%;
}

section {
  display: block;
  background-color: #fff;
  padding: 12px 24px;
  border-bottom: 1px solid #ccc;
  margin-right: 30px;
}

.variation {
  display: none;
}

.signature-attributes {
  font-size: 60%;
  color: #aaa;
  font-style: italic;
  font-weight: lighter;
}

nav {
  display: block;
  float: right;
  margin-top: 28px;
  width: 30%;
  box-sizing: border-box;
  border-left: 1px solid #ccc;
  padding-left: 16px;
}

nav ul {
  font-family: "Lucida Grande", "Lucida Sans Unicode", arial, sans-serif;
  font-size: 100%;
  line-height: 17px;
  padding: 0;
  margin: 0;
  list-style-type: none;
}

nav ul a,
nav ul a:visited,
nav ul a:active {
  font-family: Consolas, Monaco, "Andale Mono", monospace;
  line-height: 18px;
  color: #4d4e53;
}

nav h3 {
  margin-top: 12px;
}

nav li {
  margin-top: 6px;
}

footer {
  display: block;
  padding: 6px;
  margin-top: 12px;
  font-style: italic;
  font-size: 90%;
}

h1,
h2,
h3,
h4 {
  font-weight: 200;
  margin: 0;
}

h1 {
  font-family: "Open Sans Light", sans-serif;
  font-size: 48px;
  letter-spacing: -2px;
  margin: 12px 24px 20px;
}

h2,
h3.subsection-title {
  font-size: 30px;
  font-weight: 700;
  letter-spacing: -1px;
  margin-bottom: 12px;
}

h3 {
  font-size: 24px;
  letter-spacing: -0.5px;
  margin-bottom: 12px;
}

h4 {
  font-size: 18px;
  letter-spacing: -0.33px;
  margin-bottom: 12px;
  color: #4d4e53;
}

h5,
.container-overview .subsection-title {
  font-size: 120%;
  font-weight: bold;
  letter-spacing: -0.01em;
  margin: 8px 0 3px 0;
}

h6 {
  font-size: 100%;
  letter-spacing: -0.01em;
  margin: 6px 0 3px 0;
  font-style: italic;
}

table {
  border-spacing: 0;
  border: 0;
  border-collapse: collapse;
}

td,
th {
  border: 1px solid #ddd;
  margin: 0px;
  text-align: left;
  vertical-align: top;
  padding: 4px 6px;
  display: table-cell;
}

thead tr {
  background-color: #ddd;
  font-weight: bold;
}

th {
  border-right: 1px solid #aaa;
}
tr > th:last-child {
  border-right: 1px solid #ddd;
}

.ancestors,
.attribs {
  color: #999;
}
.ancestors a,
.attribs a {
  color: #999 !important;
  text-decoration: none;
}

.clear {
  clear: both;
}

.important {
  font-weight: bold;
  color: #950b02;
}

.yes-def {
  text-indent: -1000px;
}

.type-signature {
  color: #aaa;
}

.name,
.signature {
  font-family: Consolas, Monaco, "Andale Mono", monospace;
}

.details {
  margin-top: 14px;
  border-left: 2px solid #ddd;
}
.details dt {
  width: 120px;
  float: left;
  padding-left: 10px;
  padding-top: 6px;
}
.details dd {
  margin-left: 70px;
}
.details ul {
  margin: 0;
}
.details ul {
  list-style-type: none;
}
.details li {
  margin-left: 30px;
  padding-top: 6px;
}
.details pre.prettyprint {
  margin: 0;
}
.details .object-value {
  padding-top: 0;
}

.description {
  margin-bottom: 1em;
  margin-top: 1em;
}

.code-caption {
  font-style: italic;
  font-size: 107%;
  margin: 0;
}

.source {
  border: 1px solid #ddd;
  width: 80%;
  overflow: auto;
}

.prettyprint.source {
  width: inherit;
}

.source code {
  font-size: 100%;
  line-height: 18px;
  display: block;
  padding: 4px 12px;
  margin: 0;
  background-color: #fff;
  color: #4d4e53;
}

.prettyprint code span.line {
  display: inline-block;
}

.prettyprint.linenums {
  padding-left: 70px;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
}

.prettyprint.linenums ol {
  padding-left: 0;
}

.prettyprint.linenums li {
  border-left: 3px #ddd solid;
}

.prettyprint.linenums li.selected,
.prettyprint.linenums li.selected * {
  background-color: lightyellow;
}

.prettyprint.linenums li * {
  -webkit-user-select: text;
  -moz-user-select: text;
  -ms-user-select: text;
  user-select: text;
}

.params .name,
.props .name,
.name code {
  color: #4d4e53;
  font-family: Consolas, Monaco, "Andale Mono", monospace;
  font-size: 100%;
}

.params td.description > p:first-child,
.props td.description > p:first-child {
  margin-top: 0;
  padding-top: 0;
}

.params td.description > p:last-child,
.props td.description > p:last-child {
  margin-bottom: 0;
  padding-bottom: 0;
}

.disabled {
  color: #454545;
}
</style>
