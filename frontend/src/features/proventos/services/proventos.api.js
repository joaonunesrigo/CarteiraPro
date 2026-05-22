import { apiClient } from '../../carteira/services/apiClient'
import { apiUpload } from '../../carteira/services/apiUpload'

function comCarteira(caminho, carteiraId) {
  if (!carteiraId) return caminho
  const separador = caminho.includes('?') ? '&' : '?'
  return `${caminho}${separador}carteiraId=${carteiraId}`
}

export const proventosApi = {
  listar: (carteiraId) => apiClient(comCarteira('/proventos', carteiraId)),
  obterResumo: (carteiraId) => apiClient(comCarteira('/proventos/resumo', carteiraId)),
  previewImportacaoB3: (arquivo, carteiraId) => apiUpload(comCarteira('/proventos/importar/preview', carteiraId), arquivo),
  importar: (dados, carteiraId) =>
    apiClient(comCarteira('/proventos/importar', carteiraId), {
      method: 'POST',
      body: JSON.stringify(dados),
    }),
  remover: (id) => apiClient(`/proventos/${id}`, { method: 'DELETE' }),
}
