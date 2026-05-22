import { useCallback, useEffect, useMemo, useState } from 'react'
import { MOEDA_BRL } from '../constants/moedas'
import {
  useAdicionarCarteiraMutation,
  useAtualizarCarteiraMutation,
  useRemoverCarteiraMutation,
} from '../mutations/carteiraMutations'
import { useCarteirasQuery } from '../queries/carteiraQueries'
import { useCarteiraStore } from '../stores/carteiraStore'

const FORMULARIO_INICIAL = {
  nome: '',
  descricao: '',
  moeda: MOEDA_BRL,
  padrao: false,
}

export function useCarteiras(mostrarToast, solicitarConfirmacao) {
  const carteiraAtivaId = useCarteiraStore((state) => state.carteiraAtivaId)
  const setCarteiraAtivaId = useCarteiraStore((state) => state.setCarteiraAtivaId)
  const query = useCarteirasQuery()
  const adicionarCarteira = useAdicionarCarteiraMutation()
  const atualizarCarteira = useAtualizarCarteiraMutation()
  const removerCarteira = useRemoverCarteiraMutation()

  const carteiras = useMemo(() => query.data ?? [], [query.data])

  const [modalAberto, setModalAberto] = useState(false)
  const [modo, setModo] = useState('criar')
  const [formulario, setFormulario] = useState(FORMULARIO_INICIAL)
  const [erros, setErros] = useState({})

  useEffect(() => {
    if (carteiras.length === 0) return
    if (carteiraAtivaId && carteiras.some((carteira) => carteira.id === carteiraAtivaId)) return

    const padrao = carteiras.find((carteira) => carteira.padrao) ?? carteiras[0]
    setCarteiraAtivaId(padrao.id)
  }, [carteiraAtivaId, carteiras, setCarteiraAtivaId])

  const carteiraAtiva = useMemo(
    () => carteiras.find((carteira) => carteira.id === carteiraAtivaId) ?? carteiras.find((carteira) => carteira.padrao) ?? null,
    [carteiraAtivaId, carteiras],
  )

  const fecharModal = useCallback(() => {
    setModalAberto(false)
    setErros({})
  }, [])

  const abrirNovaCarteira = useCallback(() => {
    setModo('criar')
    setFormulario({ ...FORMULARIO_INICIAL, padrao: carteiras.length === 0 })
    setErros({})
    setModalAberto(true)
  }, [carteiras.length])

  const abrirEditarCarteira = useCallback(() => {
    if (!carteiraAtiva) return
    setModo('editar')
    setFormulario({
      nome: carteiraAtiva.nome,
      descricao: carteiraAtiva.descricao ?? '',
      moeda: carteiraAtiva.moeda,
      padrao: carteiraAtiva.padrao,
    })
    setErros({})
    setModalAberto(true)
  }, [carteiraAtiva])

  const atualizarCampo = useCallback(
    (campo) => (evento) => {
      const alvo = evento?.target ?? {}
      const valorBruto = alvo.type === 'checkbox' ? alvo.checked : alvo.value
      const valor = campo === 'moeda' ? Number(valorBruto) : valorBruto
      setFormulario((anterior) => ({ ...anterior, [campo]: valor }))
      setErros((anterior) => ({ ...anterior, [campo]: null }))
    },
    [],
  )

  function validar() {
    const novosErros = {}
    if (!formulario.nome?.trim()) novosErros.nome = 'Informe um nome para a carteira.'
    setErros(novosErros)
    return Object.keys(novosErros).length === 0
  }

  async function aoSalvar(evento) {
    evento.preventDefault()
    if (!validar()) return

    const dados = {
      nome: formulario.nome.trim(),
      descricao: formulario.descricao?.trim() ? formulario.descricao.trim() : null,
      moeda: Number(formulario.moeda),
      padrao: Boolean(formulario.padrao),
    }

    try {
      if (modo === 'editar' && carteiraAtiva) {
        await atualizarCarteira.mutateAsync({ id: carteiraAtiva.id, dados })
        mostrarToast?.('Carteira atualizada.', 'sucesso')
      } else {
        const resposta = await adicionarCarteira.mutateAsync(dados)
        if (resposta?.id) setCarteiraAtivaId(resposta.id)
        mostrarToast?.('Carteira criada.', 'sucesso')
      }
      fecharModal()
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao salvar a carteira.', 'erro')
    }
  }

  async function aoExcluir() {
    if (!carteiraAtiva) return

    const confirmou = await solicitarConfirmacao?.({
      titulo: 'Excluir carteira',
      mensagem: `Deseja excluir "${carteiraAtiva.nome}"? Ativos, operações e proventos vinculados serão removidos.`,
      textoConfirmar: 'Excluir',
      textoCancelar: 'Cancelar',
      variante: 'perigo',
    })

    if (!confirmou) return

    try {
      await removerCarteira.mutateAsync(carteiraAtiva.id)
      setCarteiraAtivaId(null)
      mostrarToast?.('Carteira excluída.', 'sucesso')
      fecharModal()
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao excluir carteira.', 'erro')
    }
  }

  const podeExcluir = carteiras.length > 1 && Boolean(carteiraAtiva) && !carteiraAtiva.padrao
  const enviando = adicionarCarteira.isPending || atualizarCarteira.isPending || removerCarteira.isPending

  return {
    carteiras,
    carteiraAtiva,
    carteiraAtivaId,
    carregando: query.isLoading,
    selecionarCarteira: setCarteiraAtivaId,
    abrirNovaCarteira,
    abrirEditarCarteira,
    modal: {
      aberto: modalAberto,
      modo,
      fechar: fecharModal,
    },
    formularioCarteira: {
      formulario,
      erros,
      atualizarCampo,
      enviando,
      podeExcluir,
      aoSalvar,
      aoExcluir,
    },
  }
}
