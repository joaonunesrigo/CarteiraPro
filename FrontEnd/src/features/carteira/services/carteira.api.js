import { apiClient } from './apiClient'

export const carteiraApi = {
  obterResumo: () => apiClient('/carteira/resumo'),
  obterRentabilidade: () => apiClient('/carteira/rentabilidade'),
  listarAtivos: () => apiClient('/ativos'),
  obterCotacao: (ticker) => apiClient(`/ativos/${ticker}/cotacao`),
  adicionarAtivo: (dados) =>
    apiClient('/ativos', { method: 'POST', body: JSON.stringify(dados) }),
  removerAtivo: (id) => apiClient(`/ativos/${id}`, { method: 'DELETE' }),
}
