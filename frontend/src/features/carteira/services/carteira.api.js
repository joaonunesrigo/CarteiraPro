import { apiClient } from './apiClient'
import { apiUpload } from './apiUpload'

export const carteiraApi = {
  obterResumo: () => apiClient('/carteira/resumo'),
  obterRentabilidade: () => apiClient('/carteira/rentabilidade'),
  listarAtivos: () => apiClient('/ativos'),
  obterCotacao: (ticker) => apiClient(`/ativos/${ticker}/cotacao`),
  adicionarAtivo: (dados) => apiClient('/ativos', { method: 'POST', body: JSON.stringify(dados) }),
  removerAtivo: (id) => apiClient(`/ativos/${id}`, { method: 'DELETE' }),
  removerTodosAtivos: () => apiClient('/ativos/todos', { method: 'DELETE' }),
  listarOperacoes: (ativoId) => apiClient(`/ativos/${ativoId}/operacoes`),
  adicionarOperacao: (ativoId, dados) =>
    apiClient(`/ativos/${ativoId}/operacoes`, {
      method: 'POST',
      body: JSON.stringify(dados),
    }),
  removerOperacao: (id) => apiClient(`/operacoes/${id}`, { method: 'DELETE' }),
  previewImportacaoB3: (arquivo) => apiUpload('/ativos/importar/preview', arquivo),
  importarAtivos: (dados) =>
    apiClient('/ativos/importar', {
      method: 'POST',
      body: JSON.stringify(dados),
    }),
}
