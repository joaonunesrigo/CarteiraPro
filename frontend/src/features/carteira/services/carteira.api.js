import { apiClient } from './apiClient'
import { apiUpload } from './apiUpload'

function comCarteira(caminho, carteiraId) {
  if (!carteiraId) return caminho
  const separador = caminho.includes('?') ? '&' : '?'
  return `${caminho}${separador}carteiraId=${carteiraId}`
}

export const carteiraApi = {
  listarCarteiras: () => apiClient('/carteiras'),
  adicionarCarteira: (dados) => apiClient('/carteiras', { method: 'POST', body: JSON.stringify(dados) }),
  atualizarCarteira: (id, dados) => apiClient(`/carteiras/${id}`, { method: 'PUT', body: JSON.stringify(dados) }),
  removerCarteira: (id) => apiClient(`/carteiras/${id}`, { method: 'DELETE' }),
  obterResumo: (carteiraId) => apiClient(comCarteira('/carteira/resumo', carteiraId)),
  obterRentabilidade: (carteiraId) => apiClient(comCarteira('/carteira/rentabilidade', carteiraId)),
  obterEvolucao: (meses = 12, carteiraId) => apiClient(comCarteira(`/carteira/evolucao?meses=${meses}`, carteiraId)),
  listarAtivos: (carteiraId) => apiClient(comCarteira('/ativos', carteiraId)),
  obterCotacao: (ticker) => apiClient(`/ativos/${ticker}/cotacao`),
  adicionarAtivo: (dados, carteiraId) => apiClient(comCarteira('/ativos', carteiraId), { method: 'POST', body: JSON.stringify(dados) }),
  removerAtivo: (id) => apiClient(`/ativos/${id}`, { method: 'DELETE' }),
  removerTodosAtivos: (carteiraId) => apiClient(comCarteira('/ativos/todos', carteiraId), { method: 'DELETE' }),
  listarOperacoes: (ativoId) => apiClient(`/ativos/${ativoId}/operacoes`),
  adicionarOperacao: (ativoId, dados) =>
    apiClient(`/ativos/${ativoId}/operacoes`, {
      method: 'POST',
      body: JSON.stringify(dados),
    }),
  removerOperacao: (id) => apiClient(`/operacoes/${id}`, { method: 'DELETE' }),
  previewImportacaoB3: (arquivo, carteiraId) => apiUpload(comCarteira('/ativos/importar/preview', carteiraId), arquivo),
  importarAtivos: (dados, carteiraId) =>
    apiClient(comCarteira('/ativos/importar', carteiraId), {
      method: 'POST',
      body: JSON.stringify(dados),
    }),
}
