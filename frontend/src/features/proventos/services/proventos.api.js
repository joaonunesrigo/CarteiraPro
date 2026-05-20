import { apiClient } from '../../carteira/services/apiClient'
import { apiUpload } from '../../carteira/services/apiUpload'

export const proventosApi = {
  listar: () => apiClient('/proventos'),
  obterResumo: () => apiClient('/proventos/resumo'),
  previewImportacaoB3: (arquivo) =>
    apiUpload('/proventos/importar/preview', arquivo),
  importar: (dados) =>
    apiClient('/proventos/importar', {
      method: 'POST',
      body: JSON.stringify(dados),
    }),
  remover: (id) => apiClient(`/proventos/${id}`, { method: 'DELETE' }),
}
