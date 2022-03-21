using System;
using System.Collections.Generic;
using System.Text;

namespace Harmonica.Music
{
	public class FlexibleQueue<T>
	{
		private readonly List<T> _container = new List<T>();
		private int _size => _container.Count;
		public int Size => _size; // Don't ask why, I'm on a time crunch

		public event Action? QueueChanged;
		private void InvokeQueueChanged()
		{
			QueueChanged?.Invoke();
		}

		/// <summary>
		/// Appends an element to the Queue.
		/// </summary>
		/// <param name="item"></param>
		public void Enqueue(T item)
		{
			_container.Add(item);

			InvokeQueueChanged();
		}

		/// <summary>
		/// Removes and returns the last elemenet from the Queue.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public T Dequeue()
		{
			if (_size == 0)
				throw new IndexOutOfRangeException("Queue is empty.");

			T to_return = _container[0];
			_container.RemoveAt(0);

			InvokeQueueChanged();

			return to_return;
		}

		/// <summary>
		/// Returns the last element from the Queue, without removing it.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public T Peek()
		{
			if (_size == 0)
				throw new IndexOutOfRangeException("Queue is empty.");

			return _container[_size - 1];
		}

		/// <summary>
		/// Returns true if the item is in the Queue already.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T item) => _container.Contains(item);

		public bool Empty() => _size == 0;

		/// <summary>
		/// Move the item closer to the head (place of extraction) of the Queue, swapping places with the item in front of it.
		/// If the item is already at the head, this does nothing.
		/// </summary>
		/// <param name="item"></param>
		/// <exception cref="ArgumentException"></exception>
		public void MoveUp(T item)
		{
			int index = _container.IndexOf(item);
			if(index == -1) 
				throw new ArgumentException("Item is not in queue.", nameof(item));
			
			// This is the first element in the Queue, at the HEAD
			if (index == 0) return;

			//   |-----|-----------|-------|-----|
			// 0 | ... | index - 1 | index | ... | _size - 1
			//   |-----|-----------|-------|-----|
			_container[index] = _container[index - 1];
			_container[index - 1] = item;

			InvokeQueueChanged();
		}

		/// <summary>
		/// Move the item closer to the tail of the Queue, swapping places with the item behind it.
		/// If the item is already at the tail, this does nothing.
		/// </summary>
		/// <param name="item"></param>
		/// <exception cref="ArgumentException"></exception>
		public void MoveDown(T item)
		{
			int index = _container.IndexOf(item);
			if (index == -1)
				throw new ArgumentException("Item is not in queue.", nameof(item));

			// This is the last element in the Queue, at the tail
			if (index == _size - 1) return;

			//   |-----|-------|-----------|-----|
			// 0 | ... | index | index + 1 | ... | _size - 1
			//   |-----|-------|-----------|-----|
			_container[index] = _container[index + 1];
			_container[index + 1] = item;

			InvokeQueueChanged();
		}


		/// <summary>
		/// Get and remove a random item from the queue.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public T DequeueRandom()
		{
			if (_size == 0)
				throw new IndexOutOfRangeException("Queue is empty.");

			Random r = new Random();
			int index = r.Next(0, _size);

			T to_return = _container[index];
			_container.RemoveAt(index);

			InvokeQueueChanged();

			return to_return;
		}

		public List<T> ToList()
		{
			return new List<T>(_container);
		}

		public void Clear()
		{
			_container.Clear();
			InvokeQueueChanged();
		}
	}
}
