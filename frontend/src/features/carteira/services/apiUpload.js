import { getAuthToken } from '../../auth/services/authToken'

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

export async function apiUpload(caminho, arquivo, nomeCampo = 'arquivo') {
  const dados = new FormData()
  dados.append(nomeCampo, arquivo)
  const token = getAuthToken()

  const resposta = await fetch(`${API_BASE}${caminho}`, {
    method: 'POST',
    headers: token ? { Authorization: `Bearer ${token}` } : undefined,
    body: dados,
  })

  if (!resposta.ok) {
    const texto = await resposta.text()
    const mensagem = await extrairMensagemErro(resposta, texto)
    throw new Error(mensagem)
  }

  return resposta.json()
}
