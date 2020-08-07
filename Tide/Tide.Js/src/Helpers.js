 /** @param {string|Uint8Array} input */
export function encodeBase64Url(input) {
  const text =
    typeof input === "string" ? input : Buffer.from(input).toString("base64");
  return text.replace(/\=/g, "").replace(/\+/g, "-").replace(/\//g, "_");
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