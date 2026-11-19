import { copyFile, mkdir } from "node:fs/promises";
const target = new URL("../wwwroot/lib/", import.meta.url);
await mkdir(target, { recursive: true });
for (const [from, to] of [
  ["bootstrap/dist/css/bootstrap.min.css", "bootstrap.min.css"],
  ["bootstrap/dist/js/bootstrap.bundle.min.js", "bootstrap.bundle.min.js"],
  ["bootstrap/LICENSE", "bootstrap-LICENSE.txt"],
  ["jquery/dist/jquery.min.js", "jquery.min.js"],
  ["jquery/LICENSE.txt", "jquery-LICENSE.txt"],
  ["jquery-validation/dist/jquery.validate.min.js", "jquery.validate.min.js"],
  ["jquery-validation/LICENSE.md", "jquery-validation-LICENSE.md"],
  ["jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js", "jquery.validate.unobtrusive.min.js"],
  ["jquery-validation-unobtrusive/LICENSE.txt", "jquery-validation-unobtrusive-LICENSE.txt"],
])
  await copyFile(
    new URL("node_modules/" + from, import.meta.url),
    new URL(to, target),
  );
