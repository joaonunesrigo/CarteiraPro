const API_BASE = import.meta.env.VITE_API_URL ?? '/api'

export async function apiClient(caminho, opcoes = {}) {
  const resposta = await fetch(`${API_BASE}${caminho}`, {
    headers: { 'Content-Type': 'application/json', ...opcoes.headers },
    ...opcoes,
  })

  if (!resposta.ok) {
    const texto = await resposta.text()
    throw new Error(texto || resposta.statusText)
  }

  if (resposta.status === 204) return null
  return resposta.json()
}
