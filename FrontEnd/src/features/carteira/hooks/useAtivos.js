import { useState } from 'react'
import { carteiraApi } from '../services/carteira.api'

const formularioInicial = {
  ticker: '',
  precoMedio: '',
  quantidade: '',
  tipo: 0,
}

export function useAdicionarAtivo(aoConcluir) {
  const [formulario, setFormulario] = useState(formularioInicial)
  const [enviando, setEnviando] = useState(false)
  const [erro, setErro] = useState(null)

  function atualizarCampo(campo) {
    return (evento) =>
      setFormulario((atual) => ({ ...atual, [campo]: evento.target.value }))
  }

  async function enviarFormulario(evento) {
    evento.preventDefault()
    setEnviando(true)
    setErro(null)
    try {
      await carteiraApi.adicionarAtivo({
        ticker: formulario.ticker.trim().toUpperCase(),
        precoMedio: Number(formulario.precoMedio),
        quantidade: Number(formulario.quantidade),
        tipo: Number(formulario.tipo),
      })
      setFormulario(formularioInicial)
      aoConcluir()
    } catch (err) {
      setErro(err.message)
    } finally {
      setEnviando(false)
    }
  }

  return {
    formulario,
    atualizarCampo,
    enviando,
    erro,
    enviarFormulario,
  }
}

export function useExcluirAtivo(aoConcluir) {
  async function excluirAtivo(id) {
    if (!id || !confirm('Remover este ativo?')) return
    await carteiraApi.removerAtivo(id)
    aoConcluir()
  }

  return { excluirAtivo }
}
