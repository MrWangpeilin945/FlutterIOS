export default {
  api: {
    input: './OpenAPI/swagger.json', // OpenAPIのパス
    output: {
      mode: 'split',
      target: './app/api', // 生成するAPIクライアントの出力先
      client: 'react-query', 
      import: {
        axios: 'axios', // Axiosをインポート
      },
      override: {
        mutator: {
          path: './app/utils/axiosInstance.tsx',
          name: 'axiosInstance',
        },
      },
      mock: true,
      clean: true,
  },
},
};