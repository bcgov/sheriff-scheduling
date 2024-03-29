const path = require("path");
const vueSrc = "src";

const webBaseHref = process.env.WEB_BASE_HREF || '/sheriff-scheduling/';
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
				'^/sheriff-scheduling/api': {
					target: 'https://localhost:44370',
					pathRewrite: { '^/sheriff-scheduling/api': '/api' },
					headers: {
						Connection: 'keep-alive',
						'X-Forwarded-Host': 'localhost',
						'X-Forwarded-Port': '1338'
					},
					cookiePathRewrite: {
						"/api/auth": "/sheriff-scheduling/api/auth",
						"/api/auth/signin-oidc": "/sheriff-scheduling/api/auth/signin-oidc",
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
