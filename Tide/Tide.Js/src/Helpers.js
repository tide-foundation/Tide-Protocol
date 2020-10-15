// @ts-check

/** @param {string|Uint8Array} input
 * @returns {string} */
export function encodeBase64Url(input) {
  return Buffer.from(input).toString('base64').replace(/\=/g, "").replace(/\+/g, "-").replace(/\//g, "_");
}

/** @param {string|Uint8Array|Buffer} data 
 * @returns {Buffer} */
export function decodeBase64Url(data) {
  const text = typeof data === "string" ? data : data instanceof Buffer ? data.toString("base64") : Buffer.from(data).toString("base64");
  let decoded = text.replace('_', '/').replace('-', '+');
  switch (data.length % 4) {
      case 2: decoded += "=="; break;
      case 3: decoded += "="; break;
  }
  return Buffer.from(decoded, 'base64');
}

/** @param {bigInt.BigInteger} number */
export function encodeFromBig(number) {
  return encodeBase64Url(Buffer.from(number.toArray(256).value));
}

/** @param  {...Uint8Array} buffers */
export function concat(...buffers) {
  const length = buffers.reduce((sum, buff) => buff.length + sum, 0);
  const buffer = new Uint8Array(length);
  
  let step = 0;
  for (const buff of buffers) {
    buffer.set(buff, step);
    step += buff.length;
  }

  return buffer;
}

/** @param {number} time */
export function sleep(time) {
  return new Promise(resolve => setTimeout(resolve, time));
}