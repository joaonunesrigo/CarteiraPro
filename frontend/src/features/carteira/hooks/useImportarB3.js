import { useImportarAtivosMutation } from '../mutations/carteiraMutations'
import { carteiraApi } from '../services/carteira.api'
import { useCarteiraStore } from '../stores/carteiraStore'

export function precoMedioValido(valor) {
  if (valor === '' || valor === null || valor === undefined) return false
  const numero = Number(valor)
  return Number.isFinite(numero) && numero > 0
}

function criarLinhaPreview(dados, indice) {
  return {
    id: `${dados.ticker}-${indice}`,
    ticker: dados.ticker,
    quantidade: dados.quantidade,
    precoFechamento: dados.precoFechamento ?? null,
    precoMedio: dados.precoMedio != null ? String(dados.precoMedio) : '',
    tipo: dados.tipo,
    jaCadastrado: dados.jaCadastrado,
    origemAba: dados.origemAba,
    selecionado: !dados.jaCadastrado,
  }
}

export function useImportarB3(mostrarToast, aoConcluir) {
  const importacao = useCarteiraStore((state) => state.importacaoAtivos)
  const setImportacao = useCarteiraStore((state) => state.setImportacaoAtivos)
  const atualizarLinhaImportacaoAtivo = useCarteiraStore((state) => state.atualizarLinhaImportacaoAtivo)
  const alternarAtivoSelecionadoImportacao = useCarteiraStore((state) => state.alternarAtivoSelecionadoImportacao)
  const limparImportacaoAtivos = useCarteiraStore((state) => state.limparImportacaoAtivos)
  const importarAtivos = useImportarAtivosMutation()
  const { linhas, processandoArquivo, erroArquivo, nomeArquivo } = importacao

  async function aoSelecionarArquivo(evento) {
    const arquivo = evento.target.files?.[0]
    evento.target.value = ''

    if (!arquivo) return

    setImportacao({
      erroArquivo: null,
      processandoArquivo: true,
      nomeArquivo: arquivo.name,
    })

    try {
      const resposta = await carteiraApi.previewImportacaoB3(arquivo)
      const preview = (resposta.linhas ?? []).map((dados, indice) => criarLinhaPreview(dados, indice))
      setImportacao({ linhas: preview })

      if (preview.length === 0) {
        setImportacao({ erroArquivo: 'Nenhum ativo encontrado no arquivo.' })
      }
    } catch (err) {
      setImportacao({
        linhas: [],
        erroArquivo: err.message || 'Erro ao ler o arquivo.',
      })
    } finally {
      setImportacao({ processandoArquivo: false })
    }
  }

  function atualizarLinha(id, campo, valor) {
    atualizarLinhaImportacaoAtivo(id, campo, valor)
  }

  function alternarSelecionado(id) {
    alternarAtivoSelecionadoImportacao(id)
  }

  function aplicarPrecoFechamentoComoPm() {
    setImportacao({
      linhas: linhas.map((linha) => {
        if (!linha.precoFechamento) return linha
        return {
          ...linha,
          precoMedio: String(linha.precoFechamento),
        }
      }),
    })
  }

  function limparPreview() {
    limparImportacaoAtivos()
  }

  async function confirmarImportacao() {
    const selecionadas = linhas.filter((l) => l.selecionado && !l.jaCadastrado)

    if (selecionadas.length === 0) {
      mostrarToast?.('Selecione ao menos um ativo para importar.', 'erro')
      return
    }

    const semPrecoMedio = selecionadas.filter((l) => !precoMedioValido(l.precoMedio))
    if (semPrecoMedio.length > 0) {
      mostrarToast?.(
        `Informe o preço médio dos ativos selecionados (${semPrecoMedio.length} pendente(s)).`,
        'erro',
      )
      return
    }

    const invalidas = selecionadas.filter((l) => !l.ticker || l.quantidade <= 0)
    if (invalidas.length > 0) {
      mostrarToast?.('Há linhas inválidas entre os ativos selecionados.', 'erro')
      return
    }

    try {
      const resultado = await importarAtivos.mutateAsync({
        ignorarDuplicados: true,
        ativos: selecionadas.map((l) => ({
          ticker: l.ticker,
          precoMedio: Number(l.precoMedio),
          quantidade: l.quantidade,
          tipo: l.tipo,
        })),
      })

      const { importados, ignorados, erros } = resultado
      const partes = []

      if (importados > 0) partes.push(`${importados} importado(s)`)
      if (ignorados > 0) partes.push(`${ignorados} ignorado(s)`)
      if (erros?.length > 0) partes.push(`${erros.length} erro(s)`)

      const tipo = erros?.length > 0 && importados === 0 ? 'erro' : 'sucesso'

      mostrarToast?.(partes.join(', ') || 'Importação concluída.', tipo)

      if (erros?.length > 0) {
        console.warn('Erros na importação B3:', erros)
      }

      if (importados > 0) {
        limparPreview()
        aoConcluir?.()
      }
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao importar ativos.', 'erro')
    }
  }

  const totalSelecionadas = linhas.filter((l) => l.selecionado && !l.jaCadastrado).length

  return {
    linhas,
    processandoArquivo,
    importando: importarAtivos.isPending,
    erroArquivo,
    nomeArquivo,
    totalSelecionadas,
    aoSelecionarArquivo,
    atualizarLinha,
    alternarSelecionado,
    aplicarPrecoFechamentoComoPm,
    limparPreview,
    confirmarImportacao,
  }
}
