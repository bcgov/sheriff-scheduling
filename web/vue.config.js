const crypto = require('crypto');

/**
 * The MD4 algorithm is not available anymore in Node.js 17+ (because of library SSL 3).
 * In that case, silently replace MD4 by the MD5 algorithm.
 */
try {
  crypto.createHash('md4');
} catch (e) {
  console.warn('Crypto "MD4" is not supported anymore by this Node.js version');
  const origCreateHash = crypto.createHash;
  crypto.createHash = (alg, opts) => {
    return origCreateHash(alg === 'md4' ? 'md5' : alg, opts);
  };
}

const path = require("path");
const vueSrc = "src";

const webBaseHref = process.env.WEB_BASE_HREF || '/court-admin-scheduling/';
module.exports = {
	publicPath: webBaseHref,
	configureWebpack: {
		devServer: {
			open: true,
			https: true,
			host: 'localhost',
			port: 1338,
			proxy: {
				//Development purposes, if WEB_BASE_HREF changes, this will have to change as well. 
				'^/court-admin-scheduling/api': {
					target: 'https://localhost:44370',
					pathRewrite: { '^/court-admin-scheduling/api': '/api' },
					headers: {
						Connection: 'keep-alive',
						'X-Forwarded-Host': 'localhost',
						'X-Forwarded-Port': '1338'
					},
					cookiePathRewrite: {
						"/api/auth": "/court-admin-scheduling/api/auth",
						"/api/auth/signin-oidc": "/court-admin-scheduling/api/auth/signin-oidc",
						"*": ""
					},
					changeOrigin: true
				}
			}
		},
		resolve: {
			modules: [vueSrc],
			alias: {
				"@": path.resolve(__dirname, vueSrc),
				"@assets": path.resolve(__dirname, vueSrc.concat("/assets")),
				"@components": path.resolve(__dirname, vueSrc.concat("/components")),
				"@router": path.resolve(__dirname, vueSrc.concat("/router")),
				"@store": path.resolve(__dirname, vueSrc.concat("/store")),
				"@styles": path.resolve(__dirname, vueSrc.concat("/styles")),
				"@types": path.resolve(__dirname, vueSrc.concat("/types"))
			},
			extensions: ['.ts', '.vue', '.json', '.scss', '.svg', '.png', '.jpg']
		}
	}
};
