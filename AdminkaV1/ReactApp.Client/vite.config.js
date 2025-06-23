import { fileURLToPath, URL } from 'node:url';

import { defineConfig, loadEnv } from 'vite';
import plugin from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

export default defineConfig(({ mode }) => {

    const baseFolder =
        env.APPDATA !== undefined && env.APPDATA !== ''
            ? `${env.APPDATA}/ASP.NET/https`
            : `${env.HOME}/.aspnet/https`;

    const certificateName = "AdminkaReactApp";
    const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
    const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

    if (!fs.existsSync(baseFolder)) {
        fs.mkdirSync(baseFolder, { recursive: true });
    }

    if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
        if (0 !== child_process.spawnSync('dotnet', [
            'dev-certs',
            'https',
            '--export-path',
            certFilePath,
            '--format',
            'Pem',
            '--no-password',
        ], { stdio: 'inherit', }).status) {
            throw new Error("Could not create certificate.");
        }
    }

    if (mode == 'development') {
        console.log("MODE === development");
    }

    const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
        env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7084';

    const isDebugJwt = env.ENV_DEVDEBUG === 'DEVDEBUG' && mode == 'development';
    console.log("DEVDEBUG " + (isDebugJwt ? "enabled" : "disabled"));

    var define = {
        'import.meta.env.ENV_ISDEVDEBUG': JSON.stringify(isDebugJwt)
    }
    if (isDebugJwt) {
        // eslint-disable-next-line no-undef
        const env = loadEnv(mode, process.cwd())
        console.log('Original API URL:', env.VITE_ADMINKA_API_BASE_URL)
        console.log('Replacing with:', env.VITE_DEVDEBUG_ADMINKA_API_BASE_URL)
        console.log('Original UI URL:', env.VITE_ADMINKA_UI_BASE_URL)
        console.log('Replacing with:', env.VITE_DEVDEBUG_ADMINKA_UI_BASE_URL)

        define['import.meta.env.VITE_ADMINKA_API_BASE_URL'] = JSON.stringify(env.VITE_DEVDEBUG_ADMINKA_API_BASE_URL);
        define['import.meta.env.VITE_ADMINKA_UI_BASE_URL'] = JSON.stringify(env.VITE_DEVDEBUG_ADMINKA_UI_BASE_URL);
    }


    // https://vitejs.dev/config/
    return {
        plugins: [plugin()],
        resolve: {
            alias: {
                '@': fileURLToPath(new URL('./src', import.meta.url)),
                '@setupJwtTokenMiddleware':
                    isDebugJwt
                        ? fileURLToPath(new URL('./src/setupJwtTokenMiddlewareDev.js', import.meta.url))
                        : fileURLToPath(new URL('./src/setupJwtTokenMiddlewareProd.js', import.meta.url))
            }
        },
        server: {
            proxy: {
                '^/dev': { // ^ here means start of the path
                    target,
                    secure: false
                }
            },
            port: 64165,
            https: {
                key: fs.readFileSync(keyFilePath),
                cert: fs.readFileSync(certFilePath),
            }
        },
        define: define
    }
});
