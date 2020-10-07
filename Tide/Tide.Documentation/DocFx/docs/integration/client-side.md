# Client Side

## Prerequisite

The following components are required to be set up ahead of the deployment:  
[Node.js - LTS](https://nodejs.org/en/download/ "node.js Download")

## Installation

### Via npm

In your terminal:

```
npm install tide-js --save-dev
```

In your script:

```javascript
import { TideAuthentication } from "tide-js";
```

### Via cdn

In your html:

```html
<script src="https://cdn.jsdelivr.net/npm/tide-js@1.1.41/tide.js" integrity="sha384-t1dfEweZzkkhi8Dbn3scWF5FlxPR/U/tXzXzztilLgp9veHzMaUFfQKHg1cp3Txw" crossorigin="anonymous"></script>
```

## Initializing

```javascript
// Gained... somehow?
const vendorKey = "XXX";

// The endpoint where your Tide backend is setup
const serverEndpoint = "https://vendor-backend.com/api";

// Initially selected ork nodes suggested by you
const homeOrks = ["https://ork-1.com", "https://ork-2.com", "https://ork-3.com"];

// Initialize the Tide authentication instance
const tideAuth = new TideAuthentication(vendorKey, serverEndpoint, homeOrks);
```
