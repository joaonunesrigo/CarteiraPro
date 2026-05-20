const API_BASE = import.meta.env.VITE_API_URL ?? '/api'

async function extrairMensagemErro(resposta, texto) {
  if (!texto) return resposta.statusText

  try {
    const json = JSON.parse(texto)
    return json.mensagem || texto
  } catch {
    return texto
  }
}

export async function apiClient(caminho, opcoes = {}) {
  const { signal, ...resto } = opcoes

  const resposta = await fetch(`${API_BASE}${caminho}`, {
    headers: { 'Content-Type': 'application/json', ...resto.headers },
    signal,
    ...resto,
  })

  if (!resposta.ok) {
    const texto = await resposta.text()
    const mensagem = await extrairMensagemErro(resposta, texto)
    throw new Error(mensagem)
  }

  if (resposta.status === 204) return null

  const texto = await resposta.text()
  if (!texto) return null

  return JSON.parse(texto)
}
