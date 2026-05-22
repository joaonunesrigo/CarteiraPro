import { Modal } from '../../../../components/Modal'
import { FormularioCarteira } from './FormularioCarteira'

export function ModalCarteira({ aberto, modo, onFechar, formularioCarteira }) {
  const titulo = modo === 'editar' ? 'Editar carteira' : 'Nova carteira'

  return (
    <Modal aberto={aberto} onFechar={onFechar} titulo={titulo} tamanho="md">
      <FormularioCarteira modo={modo} aoCancelar={onFechar} {...formularioCarteira} />
    </Modal>
  )
}
